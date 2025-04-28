using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HTMLeague_WebApplication.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory httpClient;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(IHttpClientFactory httpClientFactory, ILogger<RegisterModel> logger)
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
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
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
                client.BaseAddress = new Uri("https://localhost:7261/"); // API base

                // Prepare registration data
                var registerData = new
                {
                    Input.Email,
                    Input.Password,
                    Input.ConfirmPassword
                };

                // Convert to JSON
                var content = new StringContent(
                    JsonSerializer.Serialize(registerData),
                    Encoding.UTF8,
                    "application/json");

                // Call API
                var response = await client.PostAsync("api/UsersAPI/Register", content);

                if (response.IsSuccessStatusCode)
                {
                    // Registration successful, redirect to login page
                    TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                    return RedirectToPage("/Account/Login");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Registration failed: {errorContent}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                ModelState.AddModelError(string.Empty, "An error occurred during registration.");
                return Page();
            }
        }
    }
}