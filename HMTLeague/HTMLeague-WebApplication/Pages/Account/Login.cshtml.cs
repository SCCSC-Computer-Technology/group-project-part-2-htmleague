using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace HTMLeague_WebApplication.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory httpClient;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IHttpClientFactory httpClientFactory, ILogger<LoginModel> logger)
        {
            httpClient = httpClientFactory;
            _logger = logger;
        }

        
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel(); // This just simplifies validation for our form

        // The form model
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Create HTTP client
                var client = httpClient.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7261/"); // API Base

                // Prepare login data
                var loginData = new
                {
                    Input.Email,
                    Input.Password,
                    Input.RememberMe
                };

                // Convert to JSON
                var content = new StringContent(
                    JsonSerializer.Serialize(loginData),
                    Encoding.UTF8,
                    "application/json");

                // Call API
                var response = await client.PostAsync("api/UsersAPI/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    // Get user data from response
                    var userData = await response.Content.ReadFromJsonAsync<UserPreference>();

                    // Create claims for authenticated user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userData.Email),
                        new Claim("Theme", userData.Theme.ToString()),
                        new Claim("Font", userData.Font),
                        new Claim("EnabledSports", userData.EnabledSports)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = Input.RememberMe
                    };

                    // Sign in the user
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please try again.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ModelState.AddModelError(string.Empty, "An error occurred during login.");
                return Page();
            }
        }

    }
}