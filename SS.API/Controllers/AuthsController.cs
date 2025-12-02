using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.Application.Services;
using SS.Core.DTOs;

namespace SS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthsController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto == null) return BadRequest("Invalid payload");
            // by default new user is "User"
            var id = await _authService.RegisterAsync(dto, role: "User");
            return Ok(new { message = "User registered", id });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null) return Unauthorized("Invalid credentials");

            var user = await _authService.GetUserByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("User not found after login. Something went wrong.");
            
            return Ok(new
            {
                token,
                userId = user.Id,
                fullName = user.FullName,
                email = user.Email,
                role = user.Role
            });


            //return Ok(new { token });
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            var result = await _authService.GoogleLoginAsync(dto.IdToken);

            if (result == null)
                return Unauthorized("Invalid Google login");

            return Ok(new
            {
                token = result.IdToken,        
                userId = result.User.Id,
                fullName = result.User.FullName,
                email = result.User.Email,
                role = result.User.Role
            });
        }


        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var isVerified = await _authService.VerifyEmailAsync(token);
            if (!isVerified)
                return BadRequest("Invalid or expired token");

            return Ok("Email verified successfully!");
        }

        #region Resend Varification
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] EmailDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("EmailRequired");

            var user = await _authService.GetUserByEmailAsync(dto.Email);

            if (user == null)
                return BadRequest("UserNotFound");

            if (user.IsEmailVerified)
                return BadRequest("AlreadyVerified");

            var token = Guid.NewGuid().ToString("N");

            await _authService.UpdateEmailTokenAsync(user.Id, token);

            await _authService.SendVerificationEmail(dto.Email, token);

            return Ok(new { message = "Verification email sent again!" });
        }
        #endregion Resend Varification

    }
}
