using Microsoft.AspNetCore.Identity;

namespace MyApiNetCore6.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = null!;
        public string ? ProfilePicture { get; set; } = null!;
    }
}
