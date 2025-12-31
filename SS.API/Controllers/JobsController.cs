using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.Application.Services;
using SS.Core.DTOs;

namespace SS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly JobServices _jobServices;
        private readonly ILogger<JobsController> _logger;
        public JobsController(JobServices jobServices, ILogger<JobsController> logger)
        {
            _jobServices = jobServices;
            _logger = logger;
        }

        #region Get All Jobs
        [HttpGet]
        //[Authorize(Roles = "User,Admin,SuperAdmin")]
        public async Task<IActionResult> GetAllJobs()
        {
            _logger.LogInformation("Get All Jobs called.");
            try
            {
                var jobs = await _jobServices.GetAllJobsAsync();
                _logger.LogInformation("Get All Jobs successful. Retrieved {Count} jobs.", jobs.Count());
                return Ok(jobs);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all jobs");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion Get All Jobs
        
        #region Get Job By Id
        [HttpGet("{id}")]
        //[Authorize(Roles = "User,Admin,SuperAdmin")]
        public async Task<IActionResult> GetJobById(int id)
        {
            _logger.LogInformation("Get Job By Id called with Id: {Id}", id);
            try
            {
                var job = await _jobServices.GetJobByIdAsync(id);
                if (job == null)
                {
                    _logger.LogWarning("Job with Id: {Id} not found.", id);
                    return NotFound($"Job with ID {id} not found");
                }
                _logger.LogInformation("Get Job By Id successful for Id: {Id}", id);
                return Ok(job);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting job by id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion Get Job By Id
        
        #region Add Job
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> AddJob([FromBody] JobOpportunityDto jobDto)
        {
            _logger.LogInformation("Add Job called.");
            try
            {
                if (jobDto == null)
                {
                    _logger.LogWarning("Add Job failed: jobDto is null.");
                    return BadRequest("Job data is null");
                }
                await _jobServices.AddJobAsync(jobDto);
                _logger.LogInformation("Job added successfully.");
                return Ok(new { message = "Job created successfully" });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new job.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion Add Job
        
        #region Update Job
        [HttpPut]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateJob([FromBody] JobOpportunityDto jobDto)
        {
            _logger.LogInformation("Update Job called for Id: {Id}", jobDto.Id);
            try
            {
                var jobId = await _jobServices.GetJobByIdAsync(jobDto.Id);
                if (jobId == null)
                {
                    _logger.LogWarning("Delete Job failed: Job with Id: {Id} not found.", jobId);
                    return NotFound($"Job with ID {jobId} not found");
                }
                if (jobDto == null )
                {
                    _logger.LogWarning("Update Job failed: jobDto is null.");
                    return BadRequest("Job data is null");
                }
                await _jobServices.UpdateJobAsync(jobDto);
                _logger.LogInformation("Job with Id: {Id} updated successfully.", jobDto.Id);
                return Ok(new { message = "Job updated successfully" });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating job with Id: {Id}", jobDto.Id);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion Update Job
        
        #region Delete Job
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            _logger.LogInformation("Delete Job called for Id: {Id}", id);
            try
            {
                var job = await _jobServices.GetJobByIdAsync(id);
                if (job == null)
                {
                    _logger.LogWarning("Delete Job failed: Job with Id: {Id} not found.", id);
                    return NotFound($"Job with ID {id} not found");
                }
                await _jobServices.DeleteJobAsync(id);
                _logger.LogInformation("Job with Id: {Id} deleted successfully.", id);
                return Ok(new { message = "Job deleted successfully" });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting job with Id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
            
        }
        #endregion Delete Job

    }
}
