using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HTMLeague_WebApplication.Pages
{
    public class PlayersModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public PlayersModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
        }
    }
}
