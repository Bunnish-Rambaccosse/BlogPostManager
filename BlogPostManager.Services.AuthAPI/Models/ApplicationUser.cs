using Microsoft.AspNetCore.Identity;

namespace BlogPostManager.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
