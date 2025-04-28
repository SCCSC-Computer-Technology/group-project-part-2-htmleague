using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HTMLeagueGroupProjectAPI.Repositories;
using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace HTMLeague_WebApplication.Pages
{
    public class TeamsNFLModel : PageModel
    {
        private readonly HttpClient httpClient;

        public List<NflTeamSeasonStat> Teams { get; set; }
        bool isDebugging = true;

        public TeamsNFLModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7261"); // API base URL
        }

        public async Task OnGetAsync()
        {
            Teams = await httpClient.GetFromJsonAsync<List<NflTeamSeasonStat>>("/api/NFL/Teams/Stats");
            if (isDebugging)
            {
                var response = await httpClient.GetAsync("/api/NFL/Teams/Stats");
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine(json);
            }
        }
    }
}
