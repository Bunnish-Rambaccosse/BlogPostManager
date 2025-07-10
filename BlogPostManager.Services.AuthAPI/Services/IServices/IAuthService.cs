using BlogPostManager.Services.AuthAPI.Models.DTO;

namespace BlogPostManager.Services.AuthAPI.Services.IServices
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDTO registrationRequestDTO);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    }
}
