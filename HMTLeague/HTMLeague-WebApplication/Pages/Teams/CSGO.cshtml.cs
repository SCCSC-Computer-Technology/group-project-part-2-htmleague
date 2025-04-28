using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HTMLeagueGroupProjectAPI.Data.DataModels;
using Microsoft.AspNetCore.Authorization;

namespace HTMLeague_WebApplication.Pages.Teams
{
    public class TeamsCSGOModel : PageModel
    {
        private readonly HttpClient httpClient;
        public List<CsgoTeamStat> Teams { get; set; }
        bool isDebugging = true;

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; } = "Name"; // Default sort

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; } = "Ascending"; // Default sort direction

        public TeamsCSGOModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7261"); // API base URL
        }

        public async Task OnGetAsync()
        {
            try
            {
                Teams = await httpClient.GetFromJsonAsync<List<CsgoTeamStat>>("/api/CSGO/Teams/Stats");
                if (isDebugging)
                {
                    var response = await httpClient.GetAsync("/api/CSGO/Teams/Stats");
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                }

                ApplySorting();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading CSGO teams: {ex.Message}");
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
                case "Maps": // Sort by # maps
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.TotalMaps).ToList()
                        : Teams.OrderByDescending(t => t.TotalMaps).ToList();
                    break;
                case "KdDiff": // Sort by KD ratio diff
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.KdDiff).ToList()
                        : Teams.OrderByDescending(t => t.KdDiff).ToList();
                    break;
                case "Kd": // Sort by KD ratio
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.Kd).ToList()
                        : Teams.OrderByDescending(t => t.Kd).ToList();
                    break;
                case "Rating": // Sort by rating
                    Teams = SortDirection == "Ascending"
                        ? Teams.OrderBy(t => t.Rating).ToList()
                        : Teams.OrderByDescending(t => t.Rating).ToList();
                    break;
                default:
                    // Default to sorting by name
                    Teams = Teams.OrderBy(t => t.Team?.TeamName).ToList();
                    break;
            }
        }
    }
}