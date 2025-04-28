using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HTMLeagueGroupProjectAPI.Data.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HTMLeague_WebApplication.Pages.Players
{
    public class PlayersNFLModel : PageModel
    {
        private readonly HttpClient httpClient;
        public List<NflPlayerViewModel> Players { get; set; } = new List<NflPlayerViewModel>();

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; } = "Name"; // Default sort

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; } = "Ascending"; // Default sort direction

        public PlayersNFLModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7261"); // API base URL
        }

        public async Task OnGetAsync()
        {
            try
            {
                // Get all players
                var players = await httpClient.GetFromJsonAsync<List<NflPlayer>>("/api/NFL/Players");
                if (players == null)
                {
                    return;
                }

                // Get all teams (for names, etc)
                var teams = await httpClient.GetFromJsonAsync<List<NflTeam>>("/api/NFL/Teams");
                var teamLookup = teams?.ToDictionary(t => t.TeamID, t => t.TeamName) ?? new Dictionary<int, string>();

                // Get stats (because NFL player stats are in third normal form - not in one central table like CSGO/NFL)
                var passStats = await httpClient.GetFromJsonAsync<List<NflPlayerCareerPassStat>>("/api/NFL/Passes");
                var rushStats = await httpClient.GetFromJsonAsync<List<NflPlayerCareerRushStat>>("/api/NFL/Rushes");
                var receiveStats = await httpClient.GetFromJsonAsync<List<NflPlayerCareerReceiveStat>>("/api/NFL/Receives");

                // Dictionaries
                var passLookup = passStats?.ToDictionary(p => p.PlayerID, p => p) ?? new Dictionary<int, NflPlayerCareerPassStat>();
                var rushLookup = rushStats?.ToDictionary(r => r.PlayerID, r => r) ?? new Dictionary<int, NflPlayerCareerRushStat>();
                var receiveLookup = receiveStats?.ToDictionary(r => r.PlayerID, r => r) ?? new Dictionary<int, NflPlayerCareerReceiveStat>();

                // Combine data
                foreach (var player in players)
                {

                    // Check if player even has any stats. If not, we will skip
                    bool hasPassStats = passLookup.ContainsKey(player.PlayerID);
                    bool hasRushStats = rushLookup.ContainsKey(player.PlayerID);
                    bool hasReceiveStats = receiveLookup.ContainsKey(player.PlayerID);

                    // Skip if player has no team. That data is not very useful
                    if (player.TeamID == null) continue;

                    // Skip if player has no stats
                    if (!hasPassStats && !hasRushStats && !hasReceiveStats) continue;

                    string teamName = player.TeamID.HasValue && teamLookup.ContainsKey(player.TeamID.Value)
                        ? teamLookup[player.TeamID.Value]
                        : "";

                    var viewModel = new NflPlayerViewModel
                    {
                        Player = player,
                        TeamName = teamName
                    };

                    // Add stats IF they exist
                    if (passLookup.ContainsKey(player.PlayerID))
                        viewModel.PassStats = passLookup[player.PlayerID];

                    if (rushLookup.ContainsKey(player.PlayerID))
                        viewModel.RushStats = rushLookup[player.PlayerID];

                    if (receiveLookup.ContainsKey(player.PlayerID))
                        viewModel.ReceiveStats = receiveLookup[player.PlayerID];

                    Players.Add(viewModel);
                }

                ApplySorting();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading NFL players: {ex.Message}");
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
                case "Team": // Sort by team name
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.TeamName).ToList()
                        : Players.OrderByDescending(p => p.TeamName).ToList();
                    break;
                case "PassYards": // Sort by passing yards
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.PassStats?.PassingYards ?? 0).ToList()
                        : Players.OrderByDescending(p => p.PassStats?.PassingYards ?? 0).ToList();
                    break;
                case "PassTDs": // Sort by passing touchdowns
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.PassStats?.TdPasses ?? 0).ToList()
                        : Players.OrderByDescending(p => p.PassStats?.TdPasses ?? 0).ToList();
                    break;
                case "Interceptions": // Sort by interceptions
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.PassStats?.Interceptions ?? 0).ToList()
                        : Players.OrderByDescending(p => p.PassStats?.Interceptions ?? 0).ToList();
                    break;
                case "RushYards": // Sort by rushing yards
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.RushStats?.RushYards ?? 0).ToList()
                        : Players.OrderByDescending(p => p.RushStats?.RushYards ?? 0).ToList();
                    break;
                case "RushTDs": // Sort by rushing touchdowns
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.RushStats?.RushTds ?? 0).ToList()
                        : Players.OrderByDescending(p => p.RushStats?.RushTds ?? 0).ToList();
                    break;
                case "RecYards": // Sort by receiving yards
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.ReceiveStats?.ReceivingYards ?? 0).ToList()
                        : Players.OrderByDescending(p => p.ReceiveStats?.ReceivingYards ?? 0).ToList();
                    break;
                case "RecTDs": // Sort by receiving touchdowns
                    Players = SortDirection == "Ascending"
                        ? Players.OrderBy(p => p.ReceiveStats?.ReceivingTds ?? 0).ToList()
                        : Players.OrderByDescending(p => p.ReceiveStats?.ReceivingTds ?? 0).ToList();
                    break;
                default:
                    // Default to sorting by name
                    Players = Players.OrderBy(p => p.Player?.FullName).ToList();
                    break;
            }
        }

    }

    // Combine player stats
    public class NflPlayerViewModel
    {
        public NflPlayer Player { get; set; }
        public string TeamName { get; set; }
        public NflPlayerCareerPassStat PassStats { get; set; }
        public NflPlayerCareerRushStat RushStats { get; set; }
        public NflPlayerCareerReceiveStat ReceiveStats { get; set; }
    }
}