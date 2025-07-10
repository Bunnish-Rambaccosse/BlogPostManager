using BlogPostManager.Services.AuthAPI.Models.DTO;
using BlogPostManager.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogPostManager.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDTO _responseDTO;
        private readonly ILogger<AuthApiController> _logger;

        public AuthApiController(IAuthService authService, ILogger<AuthApiController> logger)
        {
            _authService = authService;
            _responseDTO = new ResponseDTO();
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", registrationRequestDTO.Email);

            var errMessage = await _authService.Register(registrationRequestDTO);

            if (!string.IsNullOrEmpty(errMessage))
            {
                _logger.LogWarning("Registration failed for email {Email}: {Error}", registrationRequestDTO.Email, errMessage);
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = errMessage;

                return BadRequest(_responseDTO);
            }
            _logger.LogInformation("Registration successful for email: {Email}", registrationRequestDTO.Email);
            return Ok(_responseDTO);
        }

        [HttpPost("login")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            _logger.LogInformation("Login attempt for Username: {Username}", loginRequestDTO.UserName);

            var loginResponse = await _authService.Login(loginRequestDTO);

            if (loginResponse.User == null)
            {
                _logger.LogWarning("Login failed for Username: {Username}", loginRequestDTO.UserName);
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Invalid Credentials";
                return Unauthorized(_responseDTO);
            }

            _logger.LogInformation("Login successful Username: {Username}", loginRequestDTO.UserName);
            _responseDTO.Result = loginResponse;

            return Ok(_responseDTO);
        }
    }
}
