using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.Application.Services;
using SS.Core.DTOs;

namespace SS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly UserProfileService _service;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<UserProfilesController> _logger;
        private readonly AuthService _authService;

        public UserProfilesController(UserProfileService service, IWebHostEnvironment env, ILogger<UserProfilesController> logger, AuthService authService)
        {
            _service = service;
            _env = env;
            _logger = logger;
            _authService = authService;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var res = await _service.GetAllAsync();
            return Ok(res);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var profile = await _service.GetByIdAsync(id);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpGet("by-user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var profile = await _service.GetByUserIdAsync(userId);

            if (profile == null)
            {
                // Load basic user info
                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null) return NotFound("User not found");

                var fallback = new UserProfileDto
                {
                    Id = 0,
                    UserId = userId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Bio = "",
                    Description = "",
                    ProfileImage = null,
                    ResumeFile = null,
                    IsActive = true
                };

                return Ok(fallback);
            }

            return Ok(profile);
            //var profile = await _service.GetByUserIdAsync(userId);
            //if (profile == null) return NotFound();
            //return Ok(profile);
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize] // require login
        public async Task<IActionResult> Create([FromForm] UserProfileFormDto dto)
        {
            _logger.LogInformation("Create profile called for user {UserId}", dto.UserId);
            try
            {
                // file handling: profile image and resume
                string profileImagePath = null;
                string resumePath = null;

                if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
                {
                    var allowedImg = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    var ext = Path.GetExtension(dto.ProfileImage.FileName).ToLowerInvariant();
                    if (!allowedImg.Contains(ext)) return BadRequest("Invalid profile image type.");
                    if (dto.ProfileImage.Length > 2 * 1024 * 1024) return BadRequest("Profile image too large (max 2MB).");

                    string folder = Path.Combine(_env.ContentRootPath, "Uploads", "Profiles");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string name = Path.GetFileNameWithoutExtension(dto.ProfileImage.FileName);
                    string safe = string.Concat(name.Split(Path.GetInvalidFileNameChars()));
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string fileName = $"{safe}_{timestamp}{ext}";
                    string fullPath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.ProfileImage.CopyToAsync(stream);

                    profileImagePath = $"/Uploads/Profiles/{fileName}";
                }

                if (dto.ResumeFile != null && dto.ResumeFile.Length > 0)
                {
                    var allowedExt = new[] { ".pdf", ".doc", ".docx" };
                    var ext = Path.GetExtension(dto.ResumeFile.FileName).ToLowerInvariant();
                    if (!allowedExt.Contains(ext)) return BadRequest("Only PDF/DOC/DOCX allowed for resume.");
                    if (dto.ResumeFile.Length > 5 * 1024 * 1024) return BadRequest("Resume too large (max 5MB).");

                    string folder = Path.Combine(_env.ContentRootPath, "Uploads", "Resumes");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string name = Path.GetFileNameWithoutExtension(dto.ResumeFile.FileName);
                    string safe = string.Concat(name.Split(Path.GetInvalidFileNameChars()));
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string fileName = $"{safe}_{timestamp}{ext}";
                    string fullPath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.ResumeFile.CopyToAsync(stream);

                    resumePath = $"/Uploads/Resumes/{fileName}";
                }

                var finalDto = new UserProfileDto
                {
                    UserId = dto.UserId,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Bio = dto.Bio,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                    LinkedIn = dto.LinkedIn,
                    GitHub = dto.GitHub,
                    LeetCode = dto.LeetCode,
                    University = dto.University,
                    Degree = dto.Degree,
                    Department = dto.Department,
                    GraduationYear = dto.GraduationYear ?? 0,
                    GPA = dto.GPA,
                    CompanyName = dto.CompanyName,
                    Position = dto.Position,
                    ExperienceFrom = dto.ExperienceFrom,
                    ExperienceTo = dto.ExperienceTo,
                    TotalExperience = dto.TotalExperience,
                    NoticePeriod = dto.NoticePeriod,
                    ProfileImage = profileImagePath,
                    ResumeFile = resumePath,
                    ResumeUrl = dto.ResumeUrl,
                    CreatedAt = DateTime.UtcNow
                };

                await _service.AddAsync(finalDto);
                return Ok(new { message = "Profile created" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating profile");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> Update([FromForm] UserProfileFormDto dto)
        {
            _logger.LogInformation("Update profile called for id {Id}", dto.Id);
            try
            {
                // get existing to keep existing paths if not provided
                var existing = await _service.GetByIdAsync(dto.Id);
                if (existing == null) return NotFound("Profile not found");

                string profileImagePath = existing.ProfileImage;
                string resumePath = existing.ResumeFile;

                if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
                {
                    var allowedImg = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    var ext = Path.GetExtension(dto.ProfileImage.FileName).ToLowerInvariant();
                    if (!allowedImg.Contains(ext)) return BadRequest("Invalid profile image type.");
                    if (dto.ProfileImage.Length > 2 * 1024 * 1024) return BadRequest("Profile image too large (max 2MB).");

                    string folder = Path.Combine(_env.ContentRootPath, "Uploads", "Profiles");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string name = Path.GetFileNameWithoutExtension(dto.ProfileImage.FileName);
                    string safe = string.Concat(name.Split(Path.GetInvalidFileNameChars()));
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string fileName = $"{safe}_{timestamp}{ext}";
                    string fullPath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.ProfileImage.CopyToAsync(stream);

                    profileImagePath = $"/Uploads/Profiles/{fileName}";

                    // delete old image
                    if (!string.IsNullOrEmpty(existing.ProfileImage))
                    {
                        string old = Path.Combine(_env.ContentRootPath, existing.ProfileImage.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                        if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
                    }
                }

                if (dto.ResumeFile != null && dto.ResumeFile.Length > 0)
                {
                    var allowedExt = new[] { ".pdf", ".doc", ".docx" };
                    var ext = Path.GetExtension(dto.ResumeFile.FileName).ToLowerInvariant();
                    if (!allowedExt.Contains(ext)) return BadRequest("Only PDF/DOC/DOCX allowed for resume.");
                    if (dto.ResumeFile.Length > 5 * 1024 * 1024) return BadRequest("Resume too large (max 5MB).");

                    string folder = Path.Combine(_env.ContentRootPath, "Uploads", "Resumes");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string name = Path.GetFileNameWithoutExtension(dto.ResumeFile.FileName);
                    string safe = string.Concat(name.Split(Path.GetInvalidFileNameChars()));
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string fileName = $"{safe}_{timestamp}{ext}";
                    string fullPath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.ResumeFile.CopyToAsync(stream);

                    resumePath = $"/Uploads/Resumes/{fileName}";

                    // delete old resume
                    if (!string.IsNullOrEmpty(existing.ResumeFile))
                    {
                        string old = Path.Combine(_env.ContentRootPath, existing.ResumeFile.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                        if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
                    }
                }

                var finalDto = new UserProfileDto
                {
                    Id = dto.Id,
                    UserId = dto.UserId,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Bio = dto.Bio,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                    LinkedIn = dto.LinkedIn,
                    GitHub = dto.GitHub,
                    LeetCode = dto.LeetCode,
                    University = dto.University,
                    Degree = dto.Degree,
                    Department = dto.Department,
                    GraduationYear = dto.GraduationYear ?? 0,
                    GPA = dto.GPA,
                    CompanyName = dto.CompanyName,
                    Position = dto.Position,
                    ExperienceFrom = dto.ExperienceFrom,
                    ExperienceTo = dto.ExperienceTo,
                    TotalExperience = dto.TotalExperience,
                    NoticePeriod = dto.NoticePeriod,
                    ProfileImage = profileImagePath,
                    ResumeFile = resumePath,
                    ResumeUrl = dto.ResumeUrl,
                    UpdatedAt = DateTime.UtcNow
                };

                await _service.UpdateAsync(finalDto);
                return Ok(new { message = "Profile updated" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")] // only admins can remove others; change as needed
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new { message = "Profile deleted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
