using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HTMLeagueGroupProjectAPI.Data.DataModels
{
    [Table("UserCredential")]
    public class UserCredential
    {
        [Key]
        [StringLength(320)]
        public string Email { get; set; }
        [Required]
        [Column(TypeName = "binary(32)")]
        public byte[] HashedPassword { get; set; }
        [Required]
        [StringLength(36)]
        [Column(TypeName = "char(36)")]
        public string Salt { get; set; }
        public virtual UserPreference Preference { get; set; }
    }

    [Table("UserPreference")]
    public class UserPreference
    {
        [Key]
        [StringLength(320)]
        public string Email { get; set; }
        [Required]
        [StringLength(80)]
        public string Font { get; set; }
        [Required]
        public int Theme { get; set; }
        [StringLength(50)]
        [Column(TypeName = "nchar(50)")]
        public string? EnabledSports { get; set; }
        public virtual UserCredential Credential { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginResponse
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public class UserPreferenceRequest
    {
        public string Font { get; set; }
        public int Theme { get; set; }
        public string? EnabledSports { get; set; }
    }
}