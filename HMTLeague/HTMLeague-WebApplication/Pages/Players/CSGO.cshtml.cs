using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace HTMLeague_WebApplication.Pages.Players
{
    public class PlayersCSGOModel : PageModel
    {
        private readonly HttpClient httpClient;
        public List<CsgoPlayerViewModel> Players { get; set; } = new List<CsgoPlayerViewModel>();
        bool isDebugging = true;

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; } = "Name"; // Default sort

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; } = "Ascending"; // Default sort direction

        public PlayersCSGOModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7261"); // API Base
        }

        public async Task OnGetAsync()
        {
            try
            {
                // Get all player stats
                var playerStats = await httpClient.GetFromJsonAsync<List<CsgoPlayerStat>>("/api/CSGO/Players/Stats");
                if (playerStats == null)
                {
                    Console.WriteLine("Players null");
                    return;
                }

                // Get all teams
                var teams = await httpClient.GetFromJsonAsync<List<CsgoTeam>>("/api/CSGO/Teams");
                var teamDict = new Dictionary<int, CsgoTeam>();
                if (teams != null)
                {
                    foreach (var team in teams)
                    {
                        teamDict[team.TeamID] = team;
                    }
                } else
                {
                    Console.WriteLine("Teams null");
                }

                // Get player team relationships
                var playerTeams = await httpClient.GetFromJsonAsync<List<CsgoPlayerTeam>>("/api/CSGO/PlayerTeams");
                var playerTeamDict = new Dictionary<int, int>();
                if (playerTeams != null)
                {
                    foreach (var pt in playerTeams)
                    {
                        playerTeamDict[pt.PlayerID] = pt.TeamID;
                    }
                } else
                {
                    Console.WriteLine("playerTeams null");
                }

                // Combine all the data
                foreach (var stat in playerStats)
                {
                    var playerViewModel = new CsgoPlayerViewModel
                    {
                        PlayerStat = stat,
                        TeamName = "",
                        TeamID = null
                    };

                    // Find team for player
                    if (playerTeamDict.TryGetValue(stat.PlayerID, out int teamID))
                    {
                        if (teamDict.TryGetValue(teamID, out CsgoTeam team))
                        {
                            playerViewModel.TeamName = team.TeamName;
                            playerViewModel.TeamID = team.TeamID;
                        }
                    }

                    Players.Add(playerViewModel);
                }

                ApplySorting();

                if (isDebugging)
                {
                    Console.WriteLine($"Loaded {Players.Count} CSGO players with team information");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading CSGO players: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void ApplySorting()
        {
            switch (SortColumn)
            {
                case "Name": // Sort by name
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.PlayerStat.Player?.PlayerName).ToList()
                        : Players.OrderByDescending(p => p.PlayerStat.Player?.PlayerName).ToList();
                    break;
                case "TeamName": // Sort by team name
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.TeamName).ToList()
                        : Players.OrderByDescending(p => p.TeamName).ToList();
                    break;
                case "Maps": // Sort by total maps played
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.PlayerStat.TotalMaps).ToList()
                        : Players.OrderByDescending(p => p.PlayerStat.TotalMaps).ToList();
                    break;
                case "Rounds": // Sort by total rounds played
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.PlayerStat.TotalRounds).ToList()
                        : Players.OrderByDescending(p => p.PlayerStat.TotalRounds).ToList();
                    break;
                case "KdDiff": // Sort by KD ratio difference
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.PlayerStat.KdDiff).ToList()
                        : Players.OrderByDescending(p => p.PlayerStat.KdDiff).ToList();
                    break;
                case "Kd": // Sort by KD ratio
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.PlayerStat.Kd).ToList()
                        : Players.OrderByDescending(p => p.PlayerStat.Kd).ToList();
                    break;
                case "Rating": // Sort by rating
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.PlayerStat.Rating).ToList()
                        : Players.OrderByDescending(p => p.PlayerStat.Rating).ToList();
                    break;
                default:
                    // Default to sorting by name
                    Players = Players.OrderBy(p => p.PlayerStat.Player?.PlayerName).ToList();
                    break;
            }
        }
    }

    // Model to combine player stats and team info
    public class CsgoPlayerViewModel
    {
        public CsgoPlayerStat PlayerStat { get; set; }
        public string TeamName { get; set; }
        public int? TeamID { get; set; }
    }
}