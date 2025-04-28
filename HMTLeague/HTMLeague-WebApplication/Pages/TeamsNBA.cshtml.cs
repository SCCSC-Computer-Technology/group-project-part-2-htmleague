using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HTMLeagueGroupProjectAPI.Repositories;
using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace HTMLeague_WebApplication.Pages
{
    public class TeamsNBAModel : PageModel
    {
        private readonly HttpClient httpClient;

        public List<NbaTeamSeasonStat> Teams { get; set; }
        bool isDebugging = true;

        public TeamsNBAModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7261"); // API base URL
        }

        public async Task OnGetAsync()
        {
            Teams = await httpClient.GetFromJsonAsync<List<NbaTeamSeasonStat>>("/api/NBA/Teams/Stats");
            if (isDebugging)
            {
                var response = await httpClient.GetAsync("/api/NBA/Teams/Stats");
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine(json);
            }
        }
    }
}
