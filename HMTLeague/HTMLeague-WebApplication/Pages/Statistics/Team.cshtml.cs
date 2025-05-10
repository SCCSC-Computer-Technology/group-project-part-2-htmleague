using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HTMLeagueGroupProjectAPI.Data.DataModels;
using System.Net.Http.Json;

namespace HTMLeague_WebApplication.Pages.Statistics
{
    public class TeamModel : PageModel
    {
        private readonly HttpClient httpClient;
        public TeamViewModel? TeamInfo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } // Team ID from URL

        [BindProperty(SupportsGet = true)]
        public string Sport { get; set; } = "CSGO"; // Default

        [BindProperty(SupportsGet = true)]
        public bool IsEditMode { get; set; } = false; // Edit mode page flag

        [BindProperty]
        public CsgoTeamStat? CsgoTeamStat { get; set; }

        [BindProperty]
        public NbaTeamSeasonStat? NbaTeamStat { get; set; }

        [BindProperty]
        public NflTeamSeasonStat? NflTeamStat { get; set; }

        public TeamModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7261");
        }

        public async Task OnGetAsync()
        {
            await LoadTeamData();
        }

        // Edit mode
        public IActionResult OnPostEdit()
        {
            IsEditMode = true;
            return RedirectToPage("./Team", new { id = Id, sport = Sport, isEditMode = true });
        }

        // Update handler for saving changes
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            bool success = false;

            if (Sport == "CSGO" && CsgoTeamStat != null)
            {
                // Update CSGO team stats
                var response = await httpClient.PutAsJsonAsync($"/api/CSGO/UpdateTeamStat/{Id}", CsgoTeamStat);
                success = response.IsSuccessStatusCode;
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response is: {responseContent} - {success}");
            }
            else if (Sport == "NBA" && NbaTeamStat != null)
            {
                // Update NBA team stats
                var response = await httpClient.PutAsJsonAsync($"/api/NBA/UpdateTeamStat/{Id}", NbaTeamStat);
                success = response.IsSuccessStatusCode;
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response is: {responseContent} - {success}");
            }
            else if (Sport == "NFL" && NflTeamStat != null)
            {
                // Update NFL team stats
                var response = await httpClient.PutAsJsonAsync($"/api/NFL/UpdateTeamStat/{Id}", NflTeamStat);
                success = response.IsSuccessStatusCode;
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response is: {responseContent} - {success}");
            }

            // Return to view mode
            return RedirectToPage("./Team", new { id = Id, sport = Sport });
        }

        // Delete handler
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            bool success = false;

            if (Sport == "CSGO")
            {
                var response = await httpClient.DeleteAsync($"/api/CSGO/DeleteTeam/{Id}");
                success = response.IsSuccessStatusCode;
            }
            else if (Sport == "NBA")
            {
                var response = await httpClient.DeleteAsync($"/api/NBA/DeleteTeam/{Id}");
                success = response.IsSuccessStatusCode;
            }
            else if (Sport == "NFL")
            {
                var response = await httpClient.DeleteAsync($"/api/NFL/DeleteTeam/{Id}");
                success = response.IsSuccessStatusCode;
            }

            // Redirect to the relevant sport teams page
            return RedirectToPage($"/Teams/{Sport}");
        }

        private async Task LoadTeamData()
        {
            bool CheckPlayersForStats = false; // Disabled for now... BAD performance issues. Too many long API calls.

            if (Sport == "NBA")
            {
                var teamStat = await httpClient.GetFromJsonAsync<NbaTeamSeasonStat>($"/api/NBA/Team/Stats/{Id}");
                var players = await TryGetAsync<List<NbaPlayer>>("/api/NBA/Players");

                TeamInfo = new TeamViewModel
                {
                    Name = teamStat?.Team?.TeamName ?? "Unknown Team",
                    StatObject = teamStat,
                    Players = players?
                        .Where(p => p.TeamID == Id)
                        .Select(p => new PlayerLink { PlayerId = p.PlayerID, Name = p.FullName, HasStats = true })
                        .ToList() ?? new List<PlayerLink>()
                };
            }
            else if (Sport == "NFL")
            {
                var teamStat = await httpClient.GetFromJsonAsync<NflTeamSeasonStat>($"/api/NFL/Team/Stats/{Id}");
                var players = await TryGetAsync<List<NflPlayer>>("/api/NFL/Players");

                var playerIdsWithStats = new HashSet<int>();

                // Only show players that have stats
                if (CheckPlayersForStats)
                {
                    var passStats = await TryGetAsync<List<NflPlayerCareerPassStat>>("/api/NFL/Passes");
                    var rushStats = await TryGetAsync<List<NflPlayerCareerRushStat>>("/api/NFL/Rushes");
                    var receiveStats = await TryGetAsync<List<NflPlayerCareerReceiveStat>>("/api/NFL/Receives");

                    // Combine all stats
                    if (passStats != null) playerIdsWithStats.UnionWith(passStats.Select(p => p.PlayerID));
                    if (rushStats != null) playerIdsWithStats.UnionWith(rushStats.Select(r => r.PlayerID));
                    if (receiveStats != null) playerIdsWithStats.UnionWith(receiveStats.Select(r => r.PlayerID));
                }

                TeamInfo = new TeamViewModel
                {
                    Name = teamStat?.Team?.TeamName ?? "Unknown Team",
                    StatObject = teamStat,
                    Players = players?
                        .Where(p => p.TeamID == Id && (playerIdsWithStats.Contains(p.PlayerID) || !CheckPlayersForStats))
                        .Select(p => new PlayerLink { PlayerId = p.PlayerID, Name = p.FullName, HasStats = true })
                        .ToList() ?? new List<PlayerLink>()
                };
            }
            else // CSGO
            {
                var teamStat = await httpClient.GetFromJsonAsync<CsgoTeamStat>($"/api/CSGO/Team/Stats/{Id}");
                var players = await TryGetAsync<List<CsgoPlayer>>("/api/CSGO/Players");
                var playerTeams = await TryGetAsync<List<CsgoPlayerTeam>>("/api/CSGO/PlayerTeams");

                List<PlayerLink> linkedPlayers = new List<PlayerLink>();

                var playerIdsWithStats = new HashSet<int>();

                if (CheckPlayersForStats)
                {
                    // Only show players that have stats
                    var playerStats = await TryGetAsync<List<CsgoPlayerStat>>("/api/CSGO/Players/Stats");
                    playerIdsWithStats = playerStats?.Select(s => s.PlayerID).ToHashSet();
                }

                if (players != null && playerTeams != null)
                {
                    var playerIds = playerTeams.Where(pt => pt.TeamID == Id).Select(pt => pt.PlayerID).ToHashSet();
                    linkedPlayers = players
                        .Where(p => playerIds.Contains(p.PlayerID) && (playerIdsWithStats.Contains(p.PlayerID) || !CheckPlayersForStats))
                        .Select(p => new PlayerLink { PlayerId = p.PlayerID, Name = p.PlayerName, HasStats = true })
                        .ToList();
                }

                TeamInfo = new TeamViewModel
                {
                    Name = teamStat?.Team?.TeamName ?? "Unknown Team",
                    StatObject = teamStat,
                    Players = linkedPlayers
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
            catch
            {
            }
            return null;
        }

        public class TeamViewModel
        {
            public string Name { get; set; }
            public object StatObject { get; set; }
            public List<PlayerLink> Players { get; set; } = new();
        }

        public class PlayerLink
        {
            public int PlayerId { get; set; }
            public string Name { get; set; }
            public bool HasStats { get; set; }
        }
    }
}