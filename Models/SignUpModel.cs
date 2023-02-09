using System.ComponentModel.DataAnnotations;

namespace MyApiNetCore6.Models
{
    public class SignUpModel
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;
        public string ? Role { get; set; }
    }
}
