using System.Diagnostics;
using HTMLeague_WebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace HTMLeague_Sports_Statistics_WebApp.Controllers
{
    public class SportsController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public SportsController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
