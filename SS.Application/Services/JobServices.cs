
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SS.Core.DTOs;
using SS.Core.Interfaces;

namespace SS.Application.Services
{
    public class JobServices
    {
        private readonly IJobRepository _jobRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<JobServices> _logger;
        public JobServices(IJobRepository jobRepository, IMapper mapper,ILogger<JobServices> logger)
        {
            _jobRepository = jobRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<JobOpportunityDto>> GetAllJobsAsync()
        {
            _logger.LogInformation("Fetching all job opportunities in Services.");
            try
            {
                //var jobs = await _jobRepository.GetAllAsync("sp_GetAllJobOpportunities", new DynamicParameters());
                var jobs = await _jobRepository.GetAllAsync("sp_GetAllActiveJobs", new DynamicParameters());
                _logger.LogInformation("Successfully fetched {Count} job opportunities in Services.", jobs.Count());

                foreach (var job in jobs)
                {
                    job.Skills = JsonConvert.DeserializeObject<List<string>>(job.SkillsJson ?? "[]");
                    job.Responsibilities = JsonConvert.DeserializeObject<List<string>>(job.ResponsibilitiesJson ?? "[]");
                }
                return _mapper.Map<IEnumerable<JobOpportunityDto>>(jobs);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all job opportunities in Services.");
                throw;
            }
            
        }

        public async Task<IEnumerable<JobOpportunityDto>> GetJobsByRoleAsync(int userId, string role)
        {
            _logger.LogInformation("Fetching job opportunities by role in Services.");
            try
            {
                var p = new DynamicParameters();
                p.Add("@UserId", userId);
                p.Add("@Role", role);

                var jobs = await _jobRepository.GetAllAsync("sp_GetJobsByRole", p);
                _logger.LogInformation("Successfully fetched {Count} job opportunities by role in Services.", jobs.Count());

                foreach (var job in jobs)
                {
                    job.Skills = JsonConvert.DeserializeObject<List<string>>(job.SkillsJson ?? "[]");
                    job.Responsibilities = JsonConvert.DeserializeObject<List<string>>(job.ResponsibilitiesJson ?? "[]");
                }

                return _mapper.Map<IEnumerable<JobOpportunityDto>>(jobs);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job opportunities by role in Services.");
                throw;
            }
            
        }
        

        public async Task<JobOpportunityDto> GetUserPageJobIdAsync(int id)
        {
            _logger.LogInformation("Fetching job opportunity with Id: {Id} in Services", id);
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                var job = await _jobRepository.GetByIdAsync("sp_GetActiveJobById", parameters);
                if (job == null)
                {
                    _logger.LogWarning("Job opportunity with Id: {Id} not found in Services.", id);
                    return null;
                }
                job.Skills = JsonConvert.DeserializeObject<List<string>>(job.SkillsJson ?? "[]");
                job.Responsibilities = JsonConvert.DeserializeObject<List<string>>(job.ResponsibilitiesJson ?? "[]");
                _logger.LogInformation("Successfully fetched job opportunity with Id: {Id} in Services", id);

                return _mapper.Map<JobOpportunityDto>(job);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job opportunity with Id: {Id} in Services", id);
                throw;
            }
        }

        public async Task<JobOpportunityDto> GetJobByIdAsync(int id,int userId,string role)
        {
            _logger.LogInformation("Fetching job opportunity with Id: {Id} in Services", id);
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@UserId", userId);
                parameters.Add("@Role", role);
                var job = await _jobRepository.GetByIdAsync("sp_GetJobOpportunityById", parameters);
                if (job == null)
                {
                    _logger.LogWarning("Job opportunity with Id: {Id} not found in Services.", id);
                    return null;
                }
                job.Skills = JsonConvert.DeserializeObject<List<string>>(job.SkillsJson ?? "[]");
                job.Responsibilities = JsonConvert.DeserializeObject<List<string>>(job.ResponsibilitiesJson ?? "[]");
                _logger.LogInformation("Successfully fetched job opportunity with Id: {Id} in Services", id);

                return _mapper.Map<JobOpportunityDto>(job);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job opportunity with Id: {Id} in Services", id);
                throw;
            }
        }
       
        public async Task AddJobAsync(JobOpportunityDto jobDto,int userId)
        {
            _logger.LogInformation("Adding new job opportunity in Services.");
            try
            {            
                var parameters = new DynamicParameters();
                parameters.Add("@Title", jobDto.Title);
                parameters.Add("@Description", jobDto.Description);
                parameters.Add("@Type", jobDto.Type);
                parameters.Add("@Experience", jobDto.Experience);
                parameters.Add("@Salary", jobDto.Salary);
                parameters.Add("@Department", jobDto.Department);
                parameters.Add("@Category", jobDto.Category);
                parameters.Add("@Image", jobDto.Image);
                parameters.Add("@Duration", jobDto.Duration);
                parameters.Add("@Location", jobDto.Location);
                parameters.Add("@CompanyName", jobDto.CompanyName);
                parameters.Add("@CreatedByUserId", userId);
                parameters.Add("@IsActive", jobDto.IsActive);
                parameters.Add("@SkillsJson", JsonConvert.SerializeObject(jobDto.Skills));
                parameters.Add("@ResponsibilitiesJson", JsonConvert.SerializeObject(jobDto.Responsibilities));
                _logger.LogInformation("Parameters prepared for new job opportunity in Services.");
                await _jobRepository.AddAsync("sp_AddJobOpportunity", parameters);
                _logger.LogInformation("New job opportunity added successfully in Services.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding new job opportunity in Services.");
                throw;
            }
        }
        public async Task UpdateJobAsync(JobOpportunityDto jobDto, int userId, string role)
        {
            _logger.LogInformation("Updating job opportunity with Id: {Id} in Services.", jobDto.Id);
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", jobDto.Id);
                parameters.Add("userId", userId);
                parameters.Add("role", role);
                parameters.Add("@Title", jobDto.Title);
                parameters.Add("@Description", jobDto.Description);
                parameters.Add("@Type", jobDto.Type);
                parameters.Add("@Experience", jobDto.Experience);
                parameters.Add("@Salary", jobDto.Salary);
                parameters.Add("@Department", jobDto.Department);
                parameters.Add("@Category", jobDto.Category);
                parameters.Add("@CompanyName", jobDto.CompanyName);
                parameters.Add("@IsActive", jobDto.IsActive);
                parameters.Add("@Image", jobDto.Image);
                parameters.Add("@Duration", jobDto.Duration);
                parameters.Add("@Location", jobDto.Location);
                parameters.Add("@SkillsJson", JsonConvert.SerializeObject(jobDto.Skills));
                parameters.Add("@ResponsibilitiesJson", JsonConvert.SerializeObject(jobDto.Responsibilities));
                _logger.LogInformation("Parameters prepared for updating job opportunity with Id: {Id} in Services.", jobDto.Id);

                await _jobRepository.UpdateAsync("sp_UpdateJobOpportunity", parameters);
                _logger.LogInformation("Job opportunity with Id: {Id} ,Title : {Title}updated successfully in Services.", jobDto.Id,jobDto.Title);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating job opportunity with Id: {Id} in Services.", jobDto.Id);
                throw;
            }
            
        }
        public async Task DeleteJobAsync(int id, int userId, string role)
        {
            _logger.LogInformation("Deleting job opportunity with Id: {Id} in Services.", id);
            try {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("userId", userId);
                parameters.Add("role", role);
                await _jobRepository.DeleteAsync("sp_DeleteJobOpportunity", parameters);
                _logger.LogInformation("Job opportunity with Id: {Id} deleted successfully in Services.", id);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting job opportunity with Id: {Id} in Services.", id);
                throw;
            }
           
        }   
    }
}


