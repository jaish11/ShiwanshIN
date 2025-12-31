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

        #region Register
        public async Task<int> RegisterAsync(RegisterDto registerDto, string role = "User", bool isGoogle = false)
        {
            var existingUser = await GetUserByEmailAsync(registerDto.Email);
            if (existingUser != null)
                throw new Exception("EmailAlreadyExists");
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
        #endregion Register

        #region Login
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
        #endregion Login

        #region Verify Email
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
        #endregion Verify Email

        #region Get User By Id
        public async Task<User> GetUserByIdAsync(int userId)
        {
            var p = new DynamicParameters();
            p.Add("@Id", userId);

            return await _userRepository.GetByIdAsync("sp_GetUserById", p);
        }
        #endregion Get User By Id

        #region Google Login
        public async Task<GoogleLoginDto> GoogleLoginAsync(string idToken)
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
            var token = GenerateJwtToken(user);
            // Return your existing JWT token
            return new GoogleLoginDto
            {
                IdToken = token,
                User = user,
            };
        }
        #endregion Google Login

        #region Generate JWT Token
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
        #endregion Generate JWT Token

        #region Send Verification Email
        public async Task SendVerificationEmail(string email, string token)
        {
            var backendUrl = _configuration["App:BackendUrl"];

            var verifyUrl = $"{backendUrl}/api/auths/verify-email?token={token}";

            //var verifyUrl = $"https://localhost:7130/api/auths/verify-email?token={token}";
            var host = _configuration["EmailSettings:Host"];
            var port = int.Parse(_configuration["EmailSettings:Port"]);
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var password = _configuration["EmailSettings:Password"];
            var displayName = _configuration["EmailSettings:DisplayName"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("OArch", "no-reply@oarch.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Verify your email";

            message.Body = new TextPart("html")
            {
                Text = $@"
                <div style='font-family:Arial, sans-serif; max-width:600px; margin:auto; padding:25px;
                                background:#ffffff; border-radius:12px; border:1px solid #e2e2e2;'>

                        <!-- Header -->
                        <h2 style='color:#0a66c2; margin-top:0; margin-bottom:8px; font-size:22px;'>
                            Verify Your Email
                        </h2>

                        <!-- Small intro -->
                        <p style='color:#555; font-size:15px; margin-top:0;'>
                            Thank you for registering with <strong>ShiwanshIn</strong>.
                            Please confirm your email address by clicking the button below:
                        </p>

                        <!-- Button -->
                        <div style='text-align:center; margin:30px 0;'>
                            <a href='{verifyUrl}'
                               style='background:#0a66c2; color:white; padding:12px 24px; 
                                      text-decoration:none; border-radius:6px; font-size:16px; display:inline-block;'>
                                Verify Email
                            </a>
                        </div>

                        <!-- Fallback link -->
                        <p style='font-size:14px; color:#666;'>
                            If the button doesn't work, copy and paste this link in your browser:
                        </p>

                        <p style='font-size:13px; color:#0a66c2; word-break:break-all; margin-top:5px;'>
                            {verifyUrl}
                        </p>

                        <!-- Divider -->
                        <hr style='margin:30px 0; border:none; border-top:1px solid #e2e2e2;' />

                        <!-- Footer -->
                        <p style='font-size:13px; color:#888; text-align:center; margin:0;'>
                            — ShiwanshIn Team
                        </p>

                 </div>"
            };



            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(fromEmail, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        #endregion Send Verification Email

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

        #region Forgot Password
        public async Task ForgotPasswordAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null || !user.IsEmailVerified) return;

            var token = Guid.NewGuid().ToString("N");
            var expiry = DateTime.Now.AddMinutes(30);
            var p = new DynamicParameters();
            p.Add("@Email", user.Email);
            p.Add("@Token", token);
            p.Add("@Expiry", expiry);
            await _userRepository.SetPasswordResetTokenAsync("sp_SetPasswordResetToken", p);
            await SendResetPasswordEmail(email, token);
        }
        #endregion Forgot Password

        #region Get User By Reset Password
        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                throw new Exception("PasswordMismatch");

            var p = new DynamicParameters();
            p.Add("@Token", dto.Token);

            var user = await _userRepository.GetByResetTokenAsync("sp_GetUserByResetToken", p);
            if (user == null) return false;
            var hash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            var update = new DynamicParameters();
            update.Add("@UserId", user.Id);
            update.Add("@PasswordHash", hash);

            await _userRepository.UpdatePasswordAsync("sp_UpdatePassword", update);
            return true;
        }
        #endregion Get User By Reset Password

        #region Change Password
        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
                return false;


            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
                return false;

            var hash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            var p = new DynamicParameters();
            p.Add("@UserId", userId);
            p.Add("@PasswordHash", hash);

            await _userRepository.UpdatePasswordAsync("sp_ChangePassword", p);
            return true;
        }
        #endregion Change Password

        #region Send Reset Password Email
        public async Task SendResetPasswordEmail(string email, string token)
        {
            var frontendUrl = _configuration["App:FrontendUrl"];
            var resetUrl = $"{frontendUrl}/reset-password?token={token}";

            var host = _configuration["EmailSettings:Host"];
            var port = int.Parse(_configuration["EmailSettings:Port"]);
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var password = _configuration["EmailSettings:Password"];
            var displayName = _configuration["EmailSettings:DisplayName"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(displayName, fromEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Reset Your Password";

            message.Body = new TextPart("html")
            {
                Text = $@"
                <div style='font-family:Arial, sans-serif; max-width:600px; margin:auto; padding:25px;
                            background:#ffffff; border-radius:12px; border:1px solid #e2e2e2;'>

                    <!-- Header -->
                    <h2 style='color:#0a66c2; margin-top:0; margin-bottom:8px; font-size:22px;'>
                        Reset Your Password
                    </h2>

                    <!-- Description -->
                    <p style='color:#555; font-size:15px; margin-top:0;'>
                        We received a request to reset your password for your
                        <strong>ShiwanshIn</strong> account.
                        Click the button below to continue.
                    </p>

                    <!-- Button -->
                    <div style='text-align:center; margin:30px 0;'>
                        <a href='{resetUrl}'
                           style='background:#0a66c2; color:white; padding:12px 24px;
                                  text-decoration:none; border-radius:6px;
                                  font-size:16px; display:inline-block;'>
                            Reset Password
                        </a>
                    </div>

                    <!-- Fallback link -->
                    <p style='font-size:14px; color:#666;'>
                        If the button doesn't work, copy and paste this link in your browser:
                    </p>

                    <p style='font-size:13px; color:#0a66c2; word-break:break-all; margin-top:5px;'>
                        {resetUrl}
                    </p>

                    <!-- Expiry -->
                    <p style='font-size:13px; color:#888;'>
                        This link will expire in <strong>30 minutes</strong>.
                    </p>

                    <hr style='margin:30px 0; border:none; border-top:1px solid #e2e2e2;' />

                    <!-- Footer -->
                    <p style='font-size:13px; color:#888; text-align:center; margin:0;'>
                        — ShiwanshIn Team
                    </p>
                </div>"
            };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(fromEmail, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        #endregion

        #region Ensure Super Admin Exists
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
        #endregion Ensure Super Admin Exists
    }
}
