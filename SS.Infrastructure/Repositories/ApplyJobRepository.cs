using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SS.Core.Entities;
using SS.Core.Interfaces;
using System.Data;

namespace SS.Infrastructure.Repositories
{
    public class ApplyJobRepository : IApplyJobRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly ILogger<ApplyJobRepository> _logger;
        public ApplyJobRepository(IConfiguration configuration,ILogger<ApplyJobRepository> logger)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlCon");
            _logger = logger;
        }
        public async Task AddAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Adding a new ApplyJob using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
                _logger.LogInformation("Successfully added a new ApplyJob using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new ApplyJob using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
                throw;
            }
            
        }

        public async Task DeleteAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Deleting an ApplyJob using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
            try {
                using var connection = new SqlConnection(_connectionString);
                await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
                _logger.LogInformation("Successfully deleted an ApplyJob using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting an ApplyJob using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
                throw;
            }
            
        }

        public async Task<IEnumerable<ApplyJob>> GetAllAsync(string sp)
        {
            _logger.LogInformation("Retrieving all ApplyJobs using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var applyJobs = await connection.QueryAsync<ApplyJob>(sp, commandType: CommandType.StoredProcedure);
                if(applyJobs == null || !applyJobs.Any())
                {
                    _logger.LogWarning("No ApplyJobs found using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
                }
                else
                {
                    _logger.LogInformation("Retrieved {Count} ApplyJobs using stored procedure: {StoredProcedure} in ApplyJobRepository.", applyJobs.Count(), sp);
                }
                return applyJobs;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all ApplyJobs using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
                throw;
            }
            
        }

        public async Task<ApplyJob> GetByIdAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Retrieving ApplyJob by Id using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var applyJob = await connection.QueryFirstOrDefaultAsync<ApplyJob>(sp, parameters, commandType: CommandType.StoredProcedure);
                if(applyJob == null)
                {
                    _logger.LogWarning("ApplyJob not found using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved ApplyJob by Id using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
                }
                return applyJob;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ApplyJob by Id using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
                throw;
            }
            
        }

        //public async Task<IEnumerable<ApplyJob>> GetByUserIdAsync(int userId)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    return await connection.QueryAsync<ApplyJob>(
        //        "sp_GetAppliedJobsByUser",
        //        new { UserId = userId },
        //        commandType: CommandType.StoredProcedure);
        //}

        public async Task<IEnumerable<ApplyJob>> GetByUserIdAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Retrieving ApplyJobs by UserId using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
            try
            {
                using var connection = new SqlConnection(_connectionString);
                _logger.LogInformation("Successfully retrieved ApplyJobs by UserId using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
                return await connection.QueryAsync<ApplyJob>(sp, parameters, commandType: CommandType.StoredProcedure);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ApplyJobs by UserId using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
                throw;
            }
            
        }


        //public async Task<bool> HasUserAppliedAsync(int userId, int jobId)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    var count = await connection.ExecuteScalarAsync<int>(
        //        "sp_CheckAlreadyApplied",
        //        new { UserId = userId, JobId = jobId },
        //        commandType: CommandType.StoredProcedure);

        //    return count > 0;
        //}
        public async Task<bool> HasUserAppliedAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Checking if user has already applied using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var count = await connection.ExecuteScalarAsync<int>(sp, parameters, commandType: CommandType.StoredProcedure);
                _logger.LogInformation("Successfully checked if user has already applied using stored procedure: {StoredProcedure} in ApplyJobRepository and Total jobs count: {count}.", sp,count);                
                return count > 0;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if user has already applied using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
                throw;
            }
        }

        public async Task UpdateAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Updating an ApplyJob using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
                _logger.LogInformation("Successfully updated an ApplyJob using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating an ApplyJob using stored procedure: {StoredProcedure} in ApplyJobRepository.", sp);
                throw;
            }
            
        }
    }
}
