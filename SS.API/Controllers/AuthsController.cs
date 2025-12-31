using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.Application.Services;
using SS.Core.DTOs;
using System.Security.Claims;

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

        #region Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto == null) return BadRequest("Invalid payload");
            // by default new user is "User"
            var id = await _authService.RegisterAsync(dto, role: "User");
            return Ok(new { message = "User registered", id });
        }
        #endregion Register

        #region Login
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
        }
        #endregion Login

        #region Google Login
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
        #endregion Google Login

        #region Email Verification
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var isVerified = await _authService.VerifyEmailAsync(token);
            if (!isVerified)
                return BadRequest("Invalid or expired token");

            return Ok("Email verified successfully!");
        }
        #endregion Email Verification

        #region Forgot Password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto.Email);
            return Ok("Password reset link sent");
        }
        #endregion Forgot Password

        #region Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var ok = await _authService.ResetPasswordAsync(dto);

            if (!ok)
                return BadRequest(new { message = "Reset link is invalid or expired" });

            return Ok(new { message = "Password reset successful" });
        }
        #endregion Reset Password

        #region Change Password
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            //var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid user token");
            }
            var ok = await _authService.ChangePasswordAsync(userId, dto);
            return ok ? Ok(new { message = "Password changed successfully" }): BadRequest(new { message = "Old password incorrect" });
        }
        #endregion Change Password

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
