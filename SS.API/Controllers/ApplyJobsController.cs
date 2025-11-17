using Microsoft.AspNetCore.Mvc;
using SS.Application.Services;
using SS.Core.DTOs;

namespace SS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplyJobsController : ControllerBase
    {
        private readonly ApplyJobServices _service;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ApplyJobsController> _logger;
        public ApplyJobsController(ApplyJobServices service, IWebHostEnvironment env, ILogger<ApplyJobsController> logger)
        {
            _service = service;
            _env = env;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllApplications()
        {
            _logger.LogInformation("Get All Applications called.");
            try
            {
                var applications = await _service.GetAllApplicationsAsync();
                _logger.LogInformation("Get All Applications successful. Retrieved {Count} applications.", applications.Count());
                return Ok(applications);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all applications");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplicationById(int id)
        {
            _logger.LogInformation("Get Application By Id called with Id: {Id}", id);
            try
            {
                var application = await _service.GetApplicationByIdAsync(id);
                if (application == null)
                {
                    _logger.LogWarning("Application with Id: {Id} not found.", id);
                    return NotFound();
                }
                _logger.LogInformation("Get Application By Id successful for Id: {Id}", id);
                return Ok(application);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting application by id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }            
        }
        #region Post Method
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddApplication([FromForm] ApplyJobFormDto dto)
        {
            _logger.LogInformation("Add Application called.");
            try
            {
                string savedResumePath = null;
                if (dto.ResumeFile != null && dto.ResumeFile.Length > 0)
                {
                    var allowedExt = new[] { ".pdf", ".doc", ".docx" };
                    var ext = Path.GetExtension(dto.ResumeFile.FileName).ToLowerInvariant();

                    if (!allowedExt.Contains(ext))
                    {
                        _logger.LogWarning("Invalid resume file extension: {Extension}", ext);
                        return BadRequest("Only PDF, DOC, DOCX allowed.");
                    }

                    if (dto.ResumeFile.Length > 5 * 1024 * 1024)
                    {
                       _logger.LogWarning("Resume file too large: {Size} bytes", dto.ResumeFile.Length);
                        return BadRequest("Resume file too large (max 5 MB).");
                    }

                    string folder = Path.Combine(_env.ContentRootPath, "Uploads", "Resumes");
                    if (!Directory.Exists(folder)) {                         
                        Directory.CreateDirectory(folder); 
                    }

                    string originalName = Path.GetFileNameWithoutExtension(dto.ResumeFile.FileName);
                    string safeName = string.Concat(originalName.Split(Path.GetInvalidFileNameChars()));

                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

                    string fileName = $"{safeName}_{timestamp}{ext}";
                    string fullPath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.ResumeFile.CopyToAsync(stream);
                    _logger.LogInformation("Resume file saved successfully at {Path}", fullPath);

                    savedResumePath = $"/Uploads/Resumes/{fileName}";
                }           

                var finalDto = new ApplyJobDto
                {
                    JobId = dto.JobId,
                    JobTitle = dto.JobTitle,
                    JobType = dto.JobType,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    ResumeUrl = dto.ResumeUrl,
                    ResumeFile = savedResumePath,
                    University = dto.University,
                    Degree = dto.Degree,
                    Department = dto.Department,
                    GPA = dto.GPA,
                    GraduationYear = dto.GraduationYear,
                    CompanyName = dto.CompanyName,
                    Position = dto.Position,
                    TotalExperience = dto.TotalExperience,
                    NoticePeriod = dto.NoticePeriod,
                    ExperienceFrom = dto.ExperienceFrom,
                    ExperienceTo = dto.ExperienceTo,
                    LinkedIn = dto.LinkedIn,
                    GitHub = dto.GitHub
                };

                await _service.AddApplicationAsync(finalDto);
                _logger.LogInformation("Application added successfully.");
                return Ok(new { message = "Applied successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding application");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion Post Method
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            _logger.LogInformation("Delete Application called for Id: {Id}", id);
            try
            {
                var existing = await _service.GetApplicationByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Delete Application failed: Application with Id: {Id} not found.", id);
                    return NotFound($"Application with ID {id} not found");
                }
                await _service.DeleteApplicationAsync(id);
                return Ok(new { message = "Application deleted successfully" });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting application with Id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
            
        }
        #region Put Method
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateApplication([FromForm] ApplyJobFormDto dto)
        {
            _logger.LogInformation("Update Application called for Id: {Id}", dto.Id);
            try
            {
                var existing = await _service.GetApplicationByIdAsync(dto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Update failed: Application Id {Id} not found.", dto.Id);
                    return NotFound("Application not found");
                }

                string savedResumePath = existing.ResumeFile;

                if (dto.ResumeFile != null && dto.ResumeFile.Length > 0)
                {
                    var allowedExt = new[] { ".pdf", ".doc", ".docx" };
                    var ext = Path.GetExtension(dto.ResumeFile.FileName).ToLowerInvariant();

                    if (!allowedExt.Contains(ext))
                    {
                        _logger.LogWarning("Invalid resume file extension: {Extension}", ext);
                        return BadRequest("Only PDF, DOC, DOCX allowed.");
                    }

                    if (dto.ResumeFile.Length > 5 * 1024 * 1024)
                    {
                        _logger.LogWarning("Resume too large: {Size} bytes", dto.ResumeFile.Length);
                        return BadRequest("Resume file too large (max 5 MB).");
                    }

                    string folder = Path.Combine(_env.ContentRootPath, "Uploads", "Resumes");
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string originalName = Path.GetFileNameWithoutExtension(dto.ResumeFile.FileName);
                    string safeName = string.Concat(originalName.Split(Path.GetInvalidFileNameChars()));

                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string fileName = $"{safeName}_{timestamp}{ext}";

                    string fullPath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.ResumeFile.CopyToAsync(stream);

                    _logger.LogInformation("Resume updated successfully at {Path}", fullPath);

                    savedResumePath = $"/Uploads/Resumes/{fileName}";

                    if (!string.IsNullOrEmpty(existing.ResumeFile))
                    {
                        string oldPath = Path.Combine(
                            _env.ContentRootPath,
                            existing.ResumeFile.Replace("/", Path.DirectorySeparatorChar.ToString())
                        );

                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                            _logger.LogInformation("Old resume deleted successfully: {OldPath}", oldPath);
                        }
                    }
                }

                var finalDto = new ApplyJobDto
                {
                    Id = dto.Id,
                    JobId = dto.JobId,
                    JobTitle = dto.JobTitle,
                    JobType = dto.JobType,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    ResumeUrl = dto.ResumeUrl,
                    ResumeFile = savedResumePath,
                    University = dto.University,
                    Degree = dto.Degree,
                    Department = dto.Department,
                    GPA = dto.GPA,
                    GraduationYear = dto.GraduationYear,
                    CompanyName = dto.CompanyName,
                    Position = dto.Position,
                    TotalExperience = dto.TotalExperience,
                    NoticePeriod = dto.NoticePeriod,
                    ExperienceFrom = dto.ExperienceFrom,
                    ExperienceTo = dto.ExperienceTo,
                    LinkedIn = dto.LinkedIn,
                    GitHub = dto.GitHub
                };

                await _service.UpdateApplicationAsync(finalDto);
                _logger.LogInformation("Application updated successfully. Id: {Id}", dto.Id);

                return Ok(new { message = "Application updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating application Id: {Id}", dto.Id);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion Put Method
    }
}
