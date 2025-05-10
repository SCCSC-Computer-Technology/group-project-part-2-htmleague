using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace HTMLeague_WebApplication.Pages.Teams
{
    public class TeamsNFLModel : PageModel
    {
        private readonly HttpClient httpClient;
        public List<NflTeamSeasonStat> Teams { get; set; }
        bool isDebugging = true;

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; } = "Name"; // Default sort

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; } = "Ascending"; // Default sort direction

        public TeamsNFLModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7261"); // API base URL
        }

        public async Task OnGetAsync()
        {
            try
            {
                Teams = await httpClient.GetFromJsonAsync<List<NflTeamSeasonStat>>("/api/NFL/Teams/Stats");
                if (isDebugging)
                {
                    var response = await httpClient.GetAsync("/api/NFL/Teams/Stats");
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                }

                ApplySorting();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading NFL teams: {ex.Message}");
            }
        }

        private void ApplySorting()
        {
            switch (SortColumn)
            {
                case "Name": // Sort by name
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.Team?.TeamName).ToList()
                        : Teams.OrderByDescending(t => t.Team?.TeamName).ToList();
                    break;
                case "Wins": // Sort by wins
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.Wins).ToList()
                        : Teams.OrderByDescending(t => t.Wins).ToList();
                    break;
                case "Losses": // Sort by losses
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.Losses).ToList()
                        : Teams.OrderByDescending(t => t.Losses).ToList();
                    break;
                case "Ties": // Sort by ties
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.Ties).ToList()
                        : Teams.OrderByDescending(t => t.Ties).ToList();
                    break;
                case "Touchdowns": // Sort by touchdowns
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.TotalTD).ToList()
                        : Teams.OrderByDescending(t => t.TotalTD).ToList();
                    break;
                default:
                    // Default to sorting by name
                    Teams = Teams.OrderBy(t => t.Team?.TeamName).ToList();
                    break;
            }
        }
    }
}