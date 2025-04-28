using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace HTMLeague_WebApplication.Pages.Teams
{
    public class TeamsNBAModel : PageModel
    {
        private readonly HttpClient httpClient;
        public List<NbaTeamSeasonStat> Teams { get; set; }
        bool isDebugging = true;

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; } = "Name"; // Default sort

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; } = "Ascending"; // Default sort direction

        public TeamsNBAModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7261"); // API base URL
        }

        public async Task OnGetAsync()
        {
            try
            {
                Teams = await httpClient.GetFromJsonAsync<List<NbaTeamSeasonStat>>("/api/NBA/Teams/Stats");
                if (isDebugging)
                {
                    var response = await httpClient.GetAsync("/api/NBA/Teams/Stats");
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                }

                ApplySorting();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading NBA teams: {ex.Message}");
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
                case "Abbr": // Sort by team abbreviation
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.Team?.TeamAbreviation).ToList()
                        : Teams.OrderByDescending(t => t.Team?.TeamAbreviation).ToList();
                    break;
                case "Points": // Sort by points
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.TotalPoints ?? 0).ToList()
                        : Teams.OrderByDescending(t => t.TotalPoints ?? 0).ToList();
                    break;
                case "FieldGoals": // Sort by field goals
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.FieldGoals ?? 0).ToList()
                        : Teams.OrderByDescending(t => t.FieldGoals ?? 0).ToList();
                    break;
                case "ThreePoints": // Sort by 3 points
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.ThreePoints ?? 0).ToList()
                        : Teams.OrderByDescending(t => t.ThreePoints ?? 0).ToList();
                    break;
                case "Assists": // Sort by assists
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.Assists ?? 0).ToList()
                        : Teams.OrderByDescending(t => t.Assists ?? 0).ToList();
                    break;
                case "Rebounds": // Sort by rebounds
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.OffensiveRebounds ?? 0).ToList()
                        : Teams.OrderByDescending(t => t.OffensiveRebounds ?? 0).ToList();
                    break;
                default:
                    // Default to sorting by name
                    Teams = Teams.OrderBy(t => t.Team?.TeamName).ToList();
                    break;
            }
        }
    }
}