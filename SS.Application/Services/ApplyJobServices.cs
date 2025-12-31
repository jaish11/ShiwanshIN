
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Logging;
using SS.Core.DTOs;
using SS.Core.Interfaces;

namespace SS.Application.Services
{
    public class ApplyJobServices
    {
        private readonly IApplyJobRepository _applyJobRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ApplyJobServices> _logger;
        public ApplyJobServices(IApplyJobRepository applyJobRepository, IMapper mapper,ILogger<ApplyJobServices> logger)
        {
            _applyJobRepository = applyJobRepository;
            _mapper = mapper;
            _logger = logger;
        }


        #region Get All Application
        public async Task<IEnumerable<ApplyJobDto>> GetAllApplicationsAsync()
        {
            _logger.LogInformation("Fetching all job applications in Services.");
            try
            {
                var applications = await _applyJobRepository.GetAllAsync("sp_GetAllAppliedJobs");
                _logger.LogInformation("Successfully fetched {Count} job applications in Services.", applications.Count());
                return _mapper.Map<IEnumerable<ApplyJobDto>>(applications);        
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all job applications in Services.");
                throw;
            }
            
        }
        #endregion Get All Application

        //public async Task<IEnumerable<ApplyJobDto>> GetMyAppliedJobsAsync(int userId)
        //{
        //    _logger.LogInformation("Fetching applied jobs for UserId: {UserId} in Services", userId);
        //    try
        //    {
        //        var data = await _applyJobRepository.GetByUserIdAsync(userId);
        //        _logger.LogInformation("Successfully fetched {Count} applied jobs for UserId: {UserId} in Services", data.Count(), userId);
        //        return _mapper.Map<IEnumerable<ApplyJobDto>>(data);
        //    }
        //    catch(Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while fetching applied jobs for UserId: {UserId} in Services", userId);
        //        throw;
        //    }

        //}
        public async Task<IEnumerable<ApplyJobDto>> GetMyAppliedJobsAsync(int userId)
        {
            _logger.LogInformation("Fetching applied jobs for UserId: {UserId} in Services", userId);
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                var data = await _applyJobRepository.GetByUserIdAsync("sp_GetAppliedJobsByUser",parameters);
                _logger.LogInformation("Successfully fetched {Count} applied jobs for UserId: {UserId} in Services", data.Count(), userId);
                return _mapper.Map<IEnumerable<ApplyJobDto>>(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching applied jobs for UserId: {UserId} in Services", userId);
                throw;
            }

        }



        #region Get Application by UserId and JobId
        public async Task<bool> HasUserAppliedJobAsync(int userId, int jobId)
        {
            _logger.LogInformation("Fetching job application for UserId: {UserId} and JobId: {JobId} in Services", userId, jobId);
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);  
                parameters.Add("@JobId", jobId);
                return await _applyJobRepository.HasUserAppliedAsync("sp_CheckAlreadyApplied",parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job application for UserId: {UserId} and JobId: {JobId} in Services", userId, jobId);
                throw;
            }
        }
        //public async Task<bool> HasUserAppliedJobAsync(int userId, int jobId)
        //{
        //    _logger.LogInformation("Fetching job application for UserId: {UserId} and JobId: {JobId} in Services", userId, jobId);
        //    try
        //    {
        //        return await _applyJobRepository.HasUserAppliedAsync(userId, jobId);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while fetching job application for UserId: {UserId} and JobId: {JobId} in Services", userId, jobId);
        //        throw;
        //    }
        //}
        #endregion Get Application by UserId and JobId

        #region Get Application by Id
        public async Task<ApplyJobDto> GetApplicationByIdAsync(int id)
        {
            _logger.LogInformation("Fetching job application with Id: {Id} in Services", id);
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                var application = await _applyJobRepository.GetByIdAsync("sp_GetAppliedJobById", parameters);
                if (application == null)
                {
                    _logger.LogWarning("Job application with Id: {Id} not found in Services.", id);
                    return null;
                }
                _logger.LogInformation("Successfully fetched job application with Id: {Id} in Services", id);
                return _mapper.Map<ApplyJobDto>(application);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job application with Id: {Id} in Services", id);
                throw;
            }
        }
        #endregion Get Application by Id

        #region Applied
        public async Task AddApplicationAsync(ApplyJobDto dto)
        {
            _logger.LogInformation("Adding new job application for JobId: {JobId} in Services", dto.JobId);
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@JobId", dto.JobId);
                parameters.Add("@JobTitle", dto.JobTitle);
                parameters.Add("@JobType", dto.JobType);
                parameters.Add("@UserId", dto.UserId);

                parameters.Add("@FullName", dto.FullName);
                parameters.Add("@Email", dto.Email);
                parameters.Add("@Phone", dto.Phone);

                parameters.Add("@ResumeUrl", dto.ResumeUrl);
                parameters.Add("@ResumeFile", dto.ResumeFile);

                parameters.Add("@University", dto.University);
                parameters.Add("@Degree", dto.Degree);
                parameters.Add("@Department", dto.Department);
                parameters.Add("@GPA", dto.GPA);
                parameters.Add("@GraduationYear", dto.GraduationYear);

                parameters.Add("@CompanyName", dto.CompanyName);
                parameters.Add("@Position", dto.Position);
                parameters.Add("@TotalExperience", dto.TotalExperience);
                parameters.Add("@NoticePeriod", dto.NoticePeriod);
                parameters.Add("@ExperienceFrom", dto.ExperienceFrom);
                parameters.Add("@ExperienceTo", dto.ExperienceTo);

                parameters.Add("@LinkedIn", dto.LinkedIn);
                parameters.Add("@GitHub", dto.GitHub);

                var alreadyApplied =await HasUserAppliedJobAsync(dto.UserId, dto.JobId);

                if (alreadyApplied)
                    throw new Exception("You have already applied for this job.");
                await _applyJobRepository.AddAsync("sp_ApplyJob", parameters);
                _logger.LogInformation("Successfully added job application for JobId: {JobId},FullName: {FullName} in Services", dto.JobId,dto.FullName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding job application for JobId: {JobId} in Services", dto.JobId);
                throw;
            }
        }
        #endregion Applied

        #region Update Application
        public async Task UpdateApplicationAsync(ApplyJobDto dto)
        {
            _logger.LogInformation("Updating job application with Id: {Id} in Services", dto.Id);
            try
                {            
                var parameters = new DynamicParameters();
                parameters.Add("@Id", dto.Id);
                parameters.Add("@JobId", dto.JobId);
                parameters.Add("@JobTitle", dto.JobTitle);
                parameters.Add("@JobType", dto.JobType);
                parameters.Add("@FullName", dto.FullName);
                parameters.Add("@Email", dto.Email);
                parameters.Add("@Phone", dto.Phone);
                parameters.Add("@ResumeUrl", dto.ResumeUrl);
                parameters.Add("@ResumeFile", dto.ResumeFile);
                parameters.Add("@University", dto.University);
                parameters.Add("@Degree", dto.Degree);
                parameters.Add("@Department", dto.Department);
                parameters.Add("@GPA", dto.GPA);
                parameters.Add("@GraduationYear", dto.GraduationYear);
                parameters.Add("@CompanyName", dto.CompanyName);
                parameters.Add("@Position", dto.Position);
                parameters.Add("@TotalExperience", dto.TotalExperience);
                parameters.Add("@NoticePeriod", dto.NoticePeriod);
                parameters.Add("@ExperienceFrom", dto.ExperienceFrom);
                parameters.Add("@ExperienceTo", dto.ExperienceTo);
                parameters.Add("@LinkedIn", dto.LinkedIn);
                parameters.Add("@GitHub", dto.GitHub);
                await _applyJobRepository.UpdateAsync("sp_UpdateAppliedJob", parameters);
                _logger.LogInformation("Successfully updated job application with Id: {Id}, FullName: {FullName} in Services.", dto.Id,dto.FullName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating job application with Id: {Id} in Services.", dto.Id);
                throw;
            }
        }
        #endregion Update Application

        #region Delete Application
        public async Task DeleteApplicationAsync(int id)
        {
            _logger.LogInformation("Deleting job application with Id: {Id} in Services.", id);
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                await _applyJobRepository.DeleteAsync("sp_DeleteAppliedJob", parameters);
                _logger.LogInformation("Successfully deleted job application with Id: {Id} in Services.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting job application with Id: {Id} in Services.", id);
                throw;
            }
        }
        #endregion Delete Application

    }
}
