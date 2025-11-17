using Microsoft.AspNetCore.Http;
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
        public JobsController(JobServices jobServices)
        {
            _jobServices = jobServices;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _jobServices.GetAllJobsAsync();
            return Ok(jobs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await _jobServices.GetJobByIdAsync(id);
            return Ok(job);
        }
        [HttpPost]
        public async Task<IActionResult> AddJob([FromBody] JobOpportunityDto jobDto)
        {
            await _jobServices.AddJobAsync(jobDto);
            return Ok(new { message = "Job created successfully" });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateJob([FromBody] JobOpportunityDto jobDto)
        {
            await _jobServices.UpdateJobAsync(jobDto);
            return Ok(new { message = "Job updated successfully" });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            await _jobServices.DeleteJobAsync(id);
            return Ok(new { message = "Job deleted successfully" });
        }
    }
}
