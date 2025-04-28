using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStatsWeb.Controllers.API;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;

namespace SportStatsWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly HttpClient httpClient;

        public AccountController(ILogger<AccountController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7168"); // Base API
        }

        // /Account/Login
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Post via API
                var response = await httpClient.PostAsJsonAsync("/api/usersapi/login", model);

                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDto>();

                    // Create claims for the authenticated user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("Theme", user.Theme.ToString()),
                        new Claim("Font", user.Font),
                        new Claim("EnabledSports", user.EnabledSports)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        RedirectUri = returnUrl
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation("User {Email} logged in.", user.Email);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Call API to register the user
                var response = await httpClient.PostAsJsonAsync("/api/usersapi/register", model);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("User created a new account with email: {Email}", model.Email);

                    // Redirect to login page
                    TempData["SuccessMessage"] = "Registration successful! Please log in to access your account.";
                    return RedirectToAction("Login");
                }

                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Registration failed: {error}");
            }

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Profile
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            try
            {
                // Get user details from API
                var response = await httpClient.GetAsync($"/api/usersapi/{email}");

                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDto>();

                    var viewModel = new UserProfileViewModel
                    {
                        Email = user.Email,
                        Theme = user.Theme,
                        Font = user.Font,
                        EnableCSGO = user.EnabledSports.Contains("CSGO"),
                        EnableNFL = user.EnabledSports.Contains("Football"),
                        EnableNBA = user.EnabledSports.Contains("Basketball")
                    };

                    return View(viewModel);
                }

                TempData["ErrorMessage"] = "Could not load user profile";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profile for user {Email}", email);
                TempData["ErrorMessage"] = "An error occurred loading your profile";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: /Account/UpdatePreferences
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdatePreferences(UserPreferenceViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Set the email from the current user
                model.Email = User.FindFirstValue(ClaimTypes.Email);

                // Call API to update preferences
                var response = await httpClient.PutAsJsonAsync("/api/usersapi/preferences", model);

                if (response.IsSuccessStatusCode)
                {
                    // Update the claims with new values
                    await UpdateUserClaims(model);

                    TempData["SuccessMessage"] = "Your preferences have been updated.";
                    return RedirectToAction("Profile");
                }

                ModelState.AddModelError(string.Empty, "Failed to update preferences");
            }

            return View("Profile", new UserProfileViewModel
            {
                Email = model.Email,
                Theme = model.Theme,
                Font = model.Font,
                EnableCSGO = model.EnableCSGO,
                EnableNFL = model.EnableNFL,
                EnableNBA = model.EnableNBA
            });
        }

        // Helper method to update claims
        private async Task UpdateUserClaims(UserPreferenceViewModel model)
        {
            var user = User as ClaimsPrincipal;
            var identity = user.Identity as ClaimsIdentity;

            // Update theme claim
            var themeClaim = identity.FindFirst("Theme");
            if (themeClaim != null)
                identity.RemoveClaim(themeClaim);
            identity.AddClaim(new Claim("Theme", model.Theme.ToString()));

            // Update font claim
            var fontClaim = identity.FindFirst("Font");
            if (fontClaim != null)
                identity.RemoveClaim(fontClaim);
            identity.AddClaim(new Claim("Font", model.Font));

            // Update enabledSports claim
            var sportsClaim = identity.FindFirst("EnabledSports");
            if (sportsClaim != null)
                identity.RemoveClaim(sportsClaim);
            identity.AddClaim(new Claim("EnabledSports", model.GetEnabledSportsString()));

            // Sign in again with the updated claims
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true });
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

    // DTO for receiving user data from API
    public class UserDto
    {
        public string Email { get; set; }
        public int Theme { get; set; }
        public string Font { get; set; }
        public string EnabledSports { get; set; }
    }

    // View Model for user profile page
    public class UserProfileViewModel
    {
        public string Email { get; set; }
        public int Theme { get; set; }
        public string Font { get; set; }
        public bool EnableCSGO { get; set; }
        public bool EnableNFL { get; set; }
        public bool EnableNBA { get; set; }
    }
}