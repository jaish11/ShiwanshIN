using Dapper;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using SS.Core.DTOs;
using SS.Core.Entities;
using SS.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SS.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<int> RegisterAsync(RegisterDto registerDto, string role = "User", bool isGoogle = false)
        {
            var token = Guid.NewGuid().ToString("N");
            var parameters = new DynamicParameters();
            parameters.Add("@Email", registerDto.Email);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            parameters.Add("@PasswordHash", hashedPassword);
            parameters.Add("@FullName", registerDto.FullName);
            parameters.Add("@Role", role);
            if (isGoogle)
            {
                parameters.Add("@IsEmailVerified", true);
                parameters.Add("@EmailVerificationToken", null);
            }
            else
            {
                parameters.Add("@IsEmailVerified", false);
                parameters.Add("@EmailVerificationToken", token);
            }
            //parameters.Add("@EmailVerificationToken", token);
            var id = await _userRepository.AddAsync("usp_AddUser", parameters);
            if (!isGoogle)
                await SendVerificationEmail(registerDto.Email, token);
            return id;
        }
        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", loginDto.Email);
            var user = await _userRepository.GetByEmailAsync("sp_GetUserByEmail", parameters);
            if (user == null) return "UserNotFound";
            if (!user.IsActive) return "InactiveUser";
            if (!user.IsEmailVerified) return "EmailNotVerified";
            var isValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isValid) return null;
            var token = GenerateJwtToken(user);
            return token;
        }
        public async Task<bool> VerifyEmailAsync(string token)
        {
            var p = new DynamicParameters();
            p.Add("@Token", token);

            var user = await _userRepository.GetByEmailTokenAsync("sp_GetUserByToken", p);
            if (user == null) return false;

            var updateParams = new DynamicParameters();
            updateParams.Add("@Id", user.Id);

            await _userRepository.MarkEmailVerifiedAsync("sp_VerifyUserEmail", updateParams);
            return true;
        }

        public async Task<string> GoogleLoginAsync(string idToken)
        {
            var googleClientId = _configuration["Google:ClientId"];

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { googleClientId }
                    });
            }
            catch
            {
                return null;
            }

            if (payload == null || string.IsNullOrWhiteSpace(payload.Email))
                return null;

            // Check DB user by email
            var p = new DynamicParameters();
            p.Add("@Email", payload.Email);
            var existing = await _userRepository.GetByEmailAsync("sp_GetUserByEmail", p);

            int userId;

            if (existing == null)
            {
                // Create account
                var registerDto = new RegisterDto
                {
                    Email = payload.Email,
                    Password = Guid.NewGuid().ToString(),
                    FullName = payload.Name ?? payload.Email,
                    Role = "User"
                };

                userId = await RegisterAsync(registerDto, "User", isGoogle: true);
            }
            else
            {
                userId = existing.Id;
            }

            // Get user again for token creation
            var getParams = new DynamicParameters();
            getParams.Add("@Email", payload.Email);
            var user = await _userRepository.GetByEmailAsync("sp_GetUserByEmail", getParams);

            if (user == null || !user.IsActive)
                return null;

            // Return your existing JWT token
            return GenerateJwtToken(user);
        }
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secret = jwtSettings.GetValue<string>("Secret");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");
            var expiryMinutes = jwtSettings.GetValue<int>("ExpiryMinutes");

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName ?? user.Email),
            new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        public async Task SendVerificationEmail(string email, string token)
        {
            var backendUrl = _configuration["App:BackendUrl"];

            var verifyUrl = $"{backendUrl}/api/auths/verify-email?token={token}";

            //var verifyUrl = $"https://localhost:7130/api/auths/verify-email?token={token}";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("OArch", "no-reply@oarch.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Verify your email";

            message.Body = new TextPart("html")
            {
                Text = $"<h2>Verify Email</h2><p>Click the link to confirm:</p><a href=\"{verifyUrl}\">Verify Email</a>"
            };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("engjaish2004@gmail.com", "ewmsmqflbjjwdmfo");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        #region resend token
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var p = new DynamicParameters();
            p.Add("@Email", email);
            return await _userRepository.GetByEmailAsync("sp_GetUserByEmail", p);
        }
        public async Task UpdateEmailTokenAsync(int id, string token)
        {
            var p = new DynamicParameters();
            p.Add("@Id", id);
            p.Add("@Token", token);

            await _userRepository.UpdateEmailTokenAsync("sp_UpdateEmailToken", p);
        }
        #endregion resend token

        public async Task EnsureSuperAdminExists(string superEmail, string superPassword)
        {
            var p = new DynamicParameters();
            p.Add("@Email", superEmail);

            var existing = await _userRepository.GetByEmailAsync("sp_GetUserByEmail", p);

            if (existing == null)
            {
                await RegisterAsync(
                    new RegisterDto
                    {
                        Email = superEmail,
                        Password = superPassword,
                        FullName = "Super Admin"
                    },
                    role: "SuperAdmin",
                    isGoogle: true
                );
            }
        }
    }
}
