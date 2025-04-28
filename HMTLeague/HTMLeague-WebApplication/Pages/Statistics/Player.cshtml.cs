using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HTMLeagueGroupProjectAPI.Data.DataModels;
using System.Net.Http.Json;

namespace HTMLeague_WebApplication.Pages.Statistics
{
    public class PlayerModel : PageModel
    {
        private readonly HttpClient httpClient;
        public PlayerViewModel? PlayerInfo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } // Edit mode page flag

        [BindProperty(SupportsGet = true)]
        public string Sport { get; set; } = "CSGO"; 

        [BindProperty(SupportsGet = true)]
        public bool IsEditMode { get; set; } = false; // Edit mode page flag

        [BindProperty]
        public CsgoPlayerStat? CsgoStat { get; set; }

        [BindProperty]
        public NbaPlayerCareerStat? NbaStat { get; set; }

        [BindProperty]
        public NflPlayerCareerPassStat? NflPassStat { get; set; }

        [BindProperty]
        public NflPlayerCareerRushStat? NflRushStat { get; set; }

        [BindProperty]
        public NflPlayerCareerReceiveStat? NflReceiveStat { get; set; }

        public PlayerModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7261");
        }

        public async Task OnGetAsync()
        {
            await LoadPlayerData();
        }

        // Edit mode handler
        public IActionResult OnPostEdit()
        {
            IsEditMode = true;
            return RedirectToPage("./Player", new { id = Id, sport = Sport, isEditMode = true });
        }

        // Update handler for saving changes
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            bool success = false;

            if (Sport == "CSGO" && CsgoStat != null)
            {
                // Update CS:GO player stats
                var response = await httpClient.PutAsJsonAsync($"/api/CSGO/UpdatePlayerStat/{Id}", CsgoStat);
                success = response.IsSuccessStatusCode;
            }
            else if (Sport == "NBA" && NbaStat != null)
            {
                // Update NBA player stats
                var response = await httpClient.PutAsJsonAsync($"/api/NBA/UpdatePlayerStat/{Id}", NbaStat);
                success = response.IsSuccessStatusCode;
            }
            else if (Sport == "NFL")
            {
                // Update NFL player stats - may need to update multiple stat types
                if (NflPassStat != null)
                {
                    var response = await httpClient.PutAsJsonAsync($"/api/NFL/UpdatePass/{Id}", NflPassStat);
                    success = response.IsSuccessStatusCode;
                }

                if (NflRushStat != null)
                {
                    var response = await httpClient.PutAsJsonAsync($"/api/NFL/UpdateRush/{Id}", NflRushStat);
                    success = response.IsSuccessStatusCode;
                }

                if (NflReceiveStat != null)
                {
                    var response = await httpClient.PutAsJsonAsync($"/api/NFL/UpdateReceive/{Id}", NflReceiveStat);
                    success = response.IsSuccessStatusCode;
                }
            }

            // Return to view mode
            return RedirectToPage("./Player", new { id = Id, sport = Sport });
        }

        // Delete handler
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            bool success = false;

            if (Sport == "CSGO")
            {
                var response = await httpClient.DeleteAsync($"/api/CSGO/DeletePlayer/{Id}");
                success = response.IsSuccessStatusCode;
            }
            else if (Sport == "NBA")
            {
                var response = await httpClient.DeleteAsync($"/api/NBA/DeletePlayer/{Id}");
                success = response.IsSuccessStatusCode;
            }
            else if (Sport == "NFL")
            {
                var response = await httpClient.DeleteAsync($"/api/NFL/DeletePlayer/{Id}");
                success = response.IsSuccessStatusCode;
            }

            // Redirect to the relevant sport players page
            return RedirectToPage($"/Players/{Sport}");
        }

        private async Task LoadPlayerData()
        {
            if (Sport == "NBA")
            {
                var stat = await httpClient.GetFromJsonAsync<NbaPlayerCareerStat>($"/api/NBA/PlayerStat/{Id}");
                var player = await httpClient.GetFromJsonAsync<NbaPlayer>($"/api/NBA/Player/{Id}");
                PlayerInfo = new PlayerViewModel
                {
                    Name = player?.FullName ?? "Unknown",
                    TeamName = player?.Team?.TeamName ?? "Free Agent",
                    TeamId = player?.TeamID,
                    StatObject = stat
                };
            }
            else if (Sport == "NFL")
            {
                var pass = await TryGetAsync<NflPlayerCareerPassStat>($"/api/NFL/Pass/{Id}");
                var rush = await TryGetAsync<NflPlayerCareerRushStat>($"/api/NFL/Rush/{Id}");
                var receive = await TryGetAsync<NflPlayerCareerReceiveStat>($"/api/NFL/Receive/{Id}");
                var player = await TryGetAsync<NflPlayer>($"/api/NFL/Player/{Id}");

                if (pass == null && rush == null && receive == null)
                {
                    PlayerInfo = null;
                }
                else
                {
                    PlayerInfo = new PlayerViewModel
                    {
                        Name = player?.FullName ?? "Unknown",
                        TeamName = player?.Team?.TeamName ?? "Free Agent",
                        TeamId = player?.TeamID,
                        StatObject = new NflPlayerStatsViewModel
                        {
                            Pass = pass,
                            Rush = rush,
                            Receive = receive
                        }
                    };
                }
            }
            else // CSGO
            {
                var stat = await httpClient.GetFromJsonAsync<CsgoPlayerStat>($"/api/CSGO/Player/Stats/{Id}");
                var player = await httpClient.GetFromJsonAsync<CsgoPlayer>($"/api/CSGO/Player/{Id}");

                // Lookup team for CSGO player (because the data is separate)
                string? teamName = "NULL";
                int? teamId = null;
                var playerTeams = await TryGetAsync<List<CsgoPlayerTeam>>("/api/CSGO/PlayerTeams");
                var teams = await TryGetAsync<List<CsgoTeam>>("/api/CSGO/Teams");

                if (player != null && playerTeams != null && teams != null)
                {
                    var playerTeam = playerTeams.FirstOrDefault(pt => pt.PlayerID == player.PlayerID);
                    if (playerTeam != null)
                    {
                        var team = teams.FirstOrDefault(t => t.TeamID == playerTeam.TeamID);
                        if (team != null)
                        {
                            teamName = team.TeamName;
                            teamId = team.TeamID;
                        }
                    }
                }

                PlayerInfo = new PlayerViewModel
                {
                    Name = player?.PlayerName ?? "Unknown",
                    TeamName = teamName ?? "Free Agent",
                    TeamId = teamId,
                    StatObject = stat
                };
            }
        }

        // This helps ensure the page doesn't crash if some data does not exist for some reason...
        private async Task<T?> TryGetAsync<T>(string url) where T : class
        {
            try
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<T>();
            }
            catch { }
            return null;
        }

        public class PlayerViewModel
        {
            public string Name { get; set; }
            public string TeamName { get; set; }
            public int? TeamId { get; set; }
            public object StatObject { get; set; }
        }

        public class NflPlayerStatsViewModel
        {
            public NflPlayerCareerPassStat? Pass { get; set; }
            public NflPlayerCareerRushStat? Rush { get; set; }
            public NflPlayerCareerReceiveStat? Receive { get; set; }
        }
    }
}