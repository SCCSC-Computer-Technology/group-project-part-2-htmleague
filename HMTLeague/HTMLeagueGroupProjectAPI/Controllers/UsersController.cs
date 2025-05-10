using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using SportStatsWeb.Data;
using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace SportStatsWeb.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersAPIController : ControllerBase
    {
        private readonly UsersDbContext context;
        private readonly ILogger<UsersAPIController> _logger;

        public UsersAPIController(UsersDbContext context, ILogger<UsersAPIController> logger)
        {
            this.context = context;
            _logger = logger;
        }

        // api/Users/Login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            var user = context.UserCredentials.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            // Verify password hash
            if (!VerifyPasswordHash(model.Password, user.HashedPassword, user.Salt))
            {
                return Unauthorized("Invalid email or password");
            }

            // Get user preferences
            var preferences = context.UserPreferences.FirstOrDefault(p => p.Email == user.Email);

            var userResponse = new
            {
                user.Email,
                Theme = preferences?.Theme ?? 0,
                Font = preferences?.Font ?? "Default",
                EnabledSports = string.IsNullOrEmpty(preferences?.EnabledSports) ? "CSGO,Football,Basketball" : preferences.EnabledSports
            };

            return Ok(userResponse);
        }

        // api/Users/Register
        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterViewModel model)
        {
            if (context.UserCredentials.Any(u => u.Email == model.Email))
            {
                return BadRequest("Email already exists");
            }

            // Create salt and hash the password
            string salt = Guid.NewGuid().ToString();
            byte[] hashedPassword = HashPassword(model.Password, salt);

            // Create new user
            var user = new UserCredential
            {
                Email = model.Email,
                HashedPassword = hashedPassword,
                Salt = salt
            };

            // Create default user preferences
            var preferences = new UserPreference
            {
                Email = model.Email,
                Font = "Default",
                Theme = 0,
                EnabledSports = "CSGO,Football,Basketball"
            };

            context.UserCredentials.Add(user);
            context.UserPreferences.Add(preferences);
            context.SaveChanges();

            return Ok(new { Email = user.Email });
        }

        // api/Users/{email}
        [HttpGet("{email}")]
        public IActionResult GetUser(string email)
        {
            var user = context.UserCredentials.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound();
            }

            var preferences = context.UserPreferences.FirstOrDefault(p => p.Email == email);

            var userResponse = new
            {
                user.Email,
                Theme = preferences?.Theme ?? 0,
                Font = preferences?.Font ?? "Default",
                EnabledSports = preferences?.EnabledSports ?? "CSGO,Football,Basketball"
            };

            return Ok(userResponse);
        }

        // Pasword hashing function
        private byte[] HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string combined = password + salt;
                byte[] bytes = Encoding.UTF8.GetBytes(combined);
                return sha256.ComputeHash(bytes);
            }
        }

        // Check hash to ensure user is legit
        private bool VerifyPasswordHash(string password, byte[] storedHash, string salt)
        {
            byte[] computedHash = HashPassword(password, salt);

            // Compare the new hash with the stored hash
            return computedHash.SequenceEqual(storedHash);
        }
    }

    // View Models for API (simplifies HttpGet/HttpPut)
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class UserPreferenceViewModel
    {
        public string Email { get; set; }
        public string Font { get; set; }
        public int Theme { get; set; }
        public bool EnableCSGO { get; set; }
        public bool EnableNFL { get; set; }
        public bool EnableNBA { get; set; }

        public string GetEnabledSportsString()
        {
            string result = "";
            if (EnableCSGO) result += "CSGO,";
            if (EnableNFL) result += "Football,";
            if (EnableNBA) result += "Basketball,";
            return result.TrimEnd(',');
        }
    }
}