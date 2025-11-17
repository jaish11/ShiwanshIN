
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Logging;
using SS.Core.DTOs;
using SS.Core.Interfaces;

namespace SS.Application.Services
{
    public class UserProfileService
    {
        private readonly IUserProfileRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(IUserProfileRepository repo, IMapper mapper, ILogger<UserProfileService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllAsync()
        {
            _logger.LogInformation("Service: GetAll user profiles");
            var result = await _repo.GetAllAsync("sp_GetAllUserProfiles");
            return _mapper.Map<IEnumerable<UserProfileDto>>(result);
        }
        public async Task<UserProfileDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Service: GetById user profile {Id}", id);
            var p = new DynamicParameters();
            p.Add("@Id", id);
            var res = await _repo.GetByIdAsync("sp_GetUserProfileById", p);
            return _mapper.Map<UserProfileDto>(res);
        }
        public async Task<UserProfileDto> GetByUserIdAsync(int userId)
        {
            _logger.LogInformation("Service: GetByUserId {UserId}", userId);
            var p = new DynamicParameters();
            p.Add("@UserId", userId);
            var res = await _repo.GetByUserIdAsync("sp_GetUserProfileByUserId", p);
            return _mapper.Map<UserProfileDto>(res);
        }
        public async Task AddAsync(UserProfileDto dto)
        {
            _logger.LogInformation("Service: Add user profile for user {UserId}", dto.UserId);
            var p = new DynamicParameters();
            p.Add("@UserId", dto.UserId);
            p.Add("@FullName", dto.FullName);
            p.Add("@Email", dto.Email);
            p.Add("@Phone", dto.Phone);
            p.Add("@Bio", dto.Bio);
            p.Add("@Description", dto.Description);
            p.Add("@IsActive", dto.IsActive);
            p.Add("@LinkedIn", dto.LinkedIn);
            p.Add("@GitHub", dto.GitHub);
            p.Add("@LeetCode", dto.LeetCode);
            p.Add("@University", dto.University);
            p.Add("@Degree", dto.Degree);
            p.Add("@Department", dto.Department);
            p.Add("@GraduationYear", dto.GraduationYear);
            p.Add("@GPA", dto.GPA);
            p.Add("@CompanyName", dto.CompanyName);
            p.Add("@Position", dto.Position);
            p.Add("@ExperienceFrom", dto.ExperienceFrom);
            p.Add("@ExperienceTo", dto.ExperienceTo);
            p.Add("@TotalExperience", dto.TotalExperience);
            p.Add("@NoticePeriod", dto.NoticePeriod);
            p.Add("@ProfileImage", dto.ProfileImage);
            p.Add("@ResumeFile", dto.ResumeFile);
            p.Add("@ResumeUrl", dto.ResumeUrl);
            p.Add("@CreatedAt", DateTime.UtcNow);
            await _repo.AddAsync("sp_AddUserProfile", p);
        }
        public async Task UpdateAsync(UserProfileDto dto)
        {
            _logger.LogInformation("Service: Update user profile id {Id}", dto.Id);
            var p = new DynamicParameters();
            p.Add("@Id", dto.Id);
            p.Add("@UserId", dto.UserId);
            p.Add("@FullName", dto.FullName);
            p.Add("@Email", dto.Email);
            p.Add("@Phone", dto.Phone);
            p.Add("@Bio", dto.Bio);
            p.Add("@Description", dto.Description);
            p.Add("@IsActive", dto.IsActive);
            p.Add("@LinkedIn", dto.LinkedIn);
            p.Add("@GitHub", dto.GitHub);
            p.Add("@LeetCode", dto.LeetCode);
            p.Add("@University", dto.University);
            p.Add("@Degree", dto.Degree);
            p.Add("@Department", dto.Department);
            p.Add("@GraduationYear", dto.GraduationYear);
            p.Add("@GPA", dto.GPA);
            p.Add("@CompanyName", dto.CompanyName);
            p.Add("@Position", dto.Position);
            p.Add("@ExperienceFrom", dto.ExperienceFrom);
            p.Add("@ExperienceTo", dto.ExperienceTo);
            p.Add("@TotalExperience", dto.TotalExperience);
            p.Add("@NoticePeriod", dto.NoticePeriod);
            p.Add("@ProfileImage", dto.ProfileImage);
            p.Add("@ResumeFile", dto.ResumeFile);
            p.Add("@ResumeUrl", dto.ResumeUrl);
            p.Add("@UpdatedAt", DateTime.UtcNow);

            await _repo.UpdateAsync("sp_UpdateUserProfile", p);
        }
        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Service: Delete user profile id {Id}", id);
            var p = new DynamicParameters();
            p.Add("@Id", id);
            await _repo.DeleteAsync("sp_DeleteUserProfile", p);
        }

    }
}
