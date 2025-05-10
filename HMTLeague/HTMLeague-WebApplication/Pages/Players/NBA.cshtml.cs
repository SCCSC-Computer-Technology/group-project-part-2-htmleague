using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HTMLeagueGroupProjectAPI.Data.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HTMLeague_WebApplication.Pages.Players
{
    public class PlayersNBAModel : PageModel
    {
        private readonly HttpClient httpClient;
        public List<NbaPlayerViewModel> Players { get; set; } = new List<NbaPlayerViewModel>();
        bool isDebugging = true;

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; } = "Name"; // Default sort

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; } = "Ascending"; // Default sort direction

        public PlayersNBAModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7261"); // API base URL
        }

        public async Task OnGetAsync()
        {
            try
            {
                // Get all players
                var players = await httpClient.GetFromJsonAsync<List<NbaPlayer>>("/api/NBA/Players");
                if (players == null)
                {
                    return;
                }

                // Get all teams for lookup
                var teams = await httpClient.GetFromJsonAsync<List<NbaTeam>>("/api/NBA/Teams");

                // Create team lookup dictionary - use a concrete class instead of anonymous type
                Dictionary<int, ExtraTeamInfo> teamLookup = new Dictionary<int, ExtraTeamInfo>();
                if (teams != null)
                {
                    foreach (var team in teams)
                    {
                        teamLookup[team.TeamID] = new ExtraTeamInfo
                        {
                            Name = team.TeamName,
                        };
                    }
                }

                // Get career stats
                var careerStats = await httpClient.GetFromJsonAsync<List<NbaPlayerCareerStat>>("/api/NBA/Players/Stats");

                // Create stats lookup dictionary
                Dictionary<int, NbaPlayerCareerStat> statsLookup = new Dictionary<int, NbaPlayerCareerStat>();
                if (careerStats != null)
                {
                    foreach (var stat in careerStats)
                    {
                        statsLookup[stat.PlayerID] = stat;
                    }
                }

                // Combine data
                foreach (var player in players)
                {

                    // Skip if player has no team. That data is not very useful
                    if (player.TeamID == null)
                        continue;

                    // Skip players without stats
                    if (!statsLookup.ContainsKey(player.PlayerID))
                        continue;

                    ExtraTeamInfo teamInfo = null;
                    if (player.TeamID.HasValue && teamLookup.ContainsKey(player.TeamID.Value))
                    {
                        teamInfo = teamLookup[player.TeamID.Value];
                    }

                    var stats = statsLookup[player.PlayerID];

                    var viewModel = new NbaPlayerViewModel
                    {
                        Player = player,
                        TeamName = teamInfo?.Name ?? "Free Agent",
                        Stats = stats
                    };

                    Players.Add(viewModel);
                }

                // Apply sorting
                ApplySorting();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading NBA players: {ex.Message}");
            }
        }

        private void ApplySorting()
        {
            switch (SortColumn)
            {
                case "Name": // Sort by name
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.Player?.FullName).ToList()
                        : Players.OrderByDescending(p => p.Player?.FullName).ToList();
                    break;
                case "TeamName": // Sort by team name
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.TeamName).ToList()
                        : Players.OrderByDescending(p => p.TeamName).ToList();
                    break;
                case "TeamAbbr": // Sort by team abbreviation
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.TeamAbbreviation).ToList()
                        : Players.OrderByDescending(p => p.TeamAbbreviation).ToList();
                    break;
                case "Points": // Sort by poonts
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.Stats?.TotalPoints ?? 0).ToList()
                        : Players.OrderByDescending(p => p.Stats?.TotalPoints ?? 0).ToList();
                    break;
                case "FieldGoals": // Sort by field goals
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.Stats?.FieldGoals ?? 0).ToList()
                        : Players.OrderByDescending(p => p.Stats?.FieldGoals ?? 0).ToList();
                    break;
                case "ThreePoints": // Sort by 3 points
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.Stats?.ThreePoints ?? 0).ToList()
                        : Players.OrderByDescending(p => p.Stats?.ThreePoints ?? 0).ToList();
                    break;
                case "Assists": // Sort by assists
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.Stats?.Assists ?? 0).ToList()
                        : Players.OrderByDescending(p => p.Stats?.Assists ?? 0).ToList();
                    break;
                case "Rebounds": // Sort by rebounds
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.Stats?.OffensiveRebounds ?? 0).ToList()
                        : Players.OrderByDescending(p => p.Stats?.OffensiveRebounds ?? 0).ToList();
                    break;
                default:
                    // Default to sorting by name
                    Players = Players.OrderBy(p => p.Player?.FullName).ToList();
                    break;
            }
        }
    }

    // Extra team info (name etc)
    public class ExtraTeamInfo
    {
        public string Name { get; set; }
    }

    // Combine player info and stats
    public class NbaPlayerViewModel
    {
        public NbaPlayer Player { get; set; }
        public string TeamName { get; set; }
        public string TeamAbbreviation { get; set; }
        public NbaPlayerCareerStat Stats { get; set; }
    }

}