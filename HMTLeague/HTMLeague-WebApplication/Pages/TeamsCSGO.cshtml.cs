using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HTMLeagueGroupProjectAPI.Repositories;
using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace HTMLeague_WebApplication.Pages
{
    public class TeamsCSGOModel : PageModel
    {
        private readonly HttpClient httpClient;

        public List<CsgoTeamStat> Teams { get; set; }
        bool isDebugging = true;

        public TeamsCSGOModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7261"); // API base URL
        }

        public async Task OnGetAsync()
        {
            Teams = await httpClient.GetFromJsonAsync<List<CsgoTeamStat>>("/api/CSGO/Teams/Stats");
            if (isDebugging)
            {
                var response = await httpClient.GetAsync("/api/CSGO/Teams/Stats");
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine(json);
            }
        }
    }
}
