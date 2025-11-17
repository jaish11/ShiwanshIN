
using AutoMapper;
using Dapper;
using Newtonsoft.Json;
using SS.Core.DTOs;
using SS.Core.Interfaces;

namespace SS.Application.Services
{
    public class JobServices
    {
        private readonly IJobRepository _jobRepository;
        private readonly IMapper _mapper;
        public JobServices(IJobRepository jobRepository, IMapper mapper)
        {
            _jobRepository = jobRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<JobOpportunityDto>> GetAllJobsAsync()
        {
            var jobs = await _jobRepository.GetAllAsync("sp_GetAllJobOpportunities");

            foreach (var job in jobs)
            {
                job.Skills = JsonConvert.DeserializeObject<List<string>>(job.SkillsJson ?? "[]");
                job.Responsibilities = JsonConvert.DeserializeObject<List<string>>(job.ResponsibilitiesJson ?? "[]");
            }

            return _mapper.Map<IEnumerable<JobOpportunityDto>>(jobs);
        }

        public async Task<JobOpportunityDto> GetJobByIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            var job = await _jobRepository.GetByIdAsync("sp_GetJobOpportunityById", parameters);
            job.Skills = JsonConvert.DeserializeObject<List<string>>(job.SkillsJson ?? "[]");
            job.Responsibilities = JsonConvert.DeserializeObject<List<string>>(job.ResponsibilitiesJson ?? "[]");

            return _mapper.Map<JobOpportunityDto>(job);
        }
        public async Task AddJobAsync(JobOpportunityDto jobDto)
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
            parameters.Add("@SkillsJson", JsonConvert.SerializeObject(jobDto.Skills));
            parameters.Add("@ResponsibilitiesJson", JsonConvert.SerializeObject(jobDto.Responsibilities));

            await _jobRepository.AddAsync("sp_AddJobOpportunity", parameters);
        }
        public async Task UpdateJobAsync(JobOpportunityDto jobDto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", jobDto.Id);
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
            parameters.Add("@SkillsJson", JsonConvert.SerializeObject(jobDto.Skills));
            parameters.Add("@ResponsibilitiesJson", JsonConvert.SerializeObject(jobDto.Responsibilities));

            await _jobRepository.UpdateAsync("sp_UpdateJobOpportunity", parameters);
        }
        public async Task DeleteJobAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            await _jobRepository.DeleteAsync("sp_DeleteJobOpportunity", parameters);
        }   
    }
}


