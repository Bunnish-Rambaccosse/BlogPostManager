using BlogPostManager.Services.AuthAPI.Models;

namespace BlogPostManager.Services.AuthAPI.Services.IServices
{
    public interface IJwtGenerateToken
    {
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
