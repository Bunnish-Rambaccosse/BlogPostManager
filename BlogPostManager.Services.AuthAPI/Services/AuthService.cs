using BlogPostManager.Services.AuthAPI.Data;
using BlogPostManager.Services.AuthAPI.Models;
using BlogPostManager.Services.AuthAPI.Models.DTO;
using BlogPostManager.Services.AuthAPI.Services.IServices;
using BlogPostManager.Services.AuthAPI.Utility;
using Microsoft.AspNetCore.Identity;

namespace BlogPostManager.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtGenerateToken _jwtGenerateToken;

        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtGenerateToken jwtGenerateToken)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtGenerateToken = jwtGenerateToken;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.NormalizedEmail == loginRequestDTO.UserName.ToUpper());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (user == null || isValid == false)
            {
                return new LoginResponseDTO() { User = null, Token = "" };
            }

            // generate token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtGenerateToken.GenerateToken(user, roles);

            UserDTO userDTO = new UserDTO()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                User = userDTO,
                Token = token

            };

            return loginResponseDTO;
        }

        public async Task<string> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDTO.Email,
                Email = registrationRequestDTO.Email,
                PhoneNumber = registrationRequestDTO.PhoneNumber,
                Name = registrationRequestDTO.Name,
                NormalizedEmail = registrationRequestDTO.Email.ToUpper()
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(registrationRequestDTO.Role)) 
                    { 
                        registrationRequestDTO.Role = AppConstants.RoleCustomer;
                    }

                    var isAssigned = await AssignRole(registrationRequestDTO.Email, registrationRequestDTO.Role);
                    if (!isAssigned)
                    {
                        return "Role assignment failed";
                    }

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {

                //throw;
            }

            return "Error Encountered";
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

    }
}
