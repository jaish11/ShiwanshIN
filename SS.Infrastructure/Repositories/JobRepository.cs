using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SS.Core.Entities;
using SS.Core.Interfaces;
using System.Data;

namespace SS.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly ILogger<JobRepository> _logger;
        public JobRepository(IConfiguration configuration,ILogger<JobRepository> logger)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlCon");
            _logger = logger;
        }

        public async Task<IEnumerable<JobOpportunity>> GetAllAsync(string sp)
        {        
            _logger.LogInformation("Retrieving all JobOpportunities using stored procedure: {StoredProcedure} in JobRepository.", sp);
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var result = await connection.QueryAsync<JobOpportunity>(sp, commandType: CommandType.StoredProcedure); 
                if(result == null || !result.Any())
                {
                    _logger.LogWarning("No JobOpportunities found using stored procedure: {StoredProcedure} in JobRepository.", sp);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved {Count} JobOpportunities using stored procedure: {StoredProcedure} in JobRepository.", result.Count(), sp);
                }
                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all JobOpportunities using stored procedure: {StoredProcedure} in JobRepository.", sp);
                throw;
            }
            
        }

        public async Task<JobOpportunity> GetByIdAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Retrieving a JobOpportunity by Id using stored procedure: {StoredProcedure} in JobRepository.", sp);
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var result = await connection.QueryFirstOrDefaultAsync<JobOpportunity>(sp, parameters, commandType: CommandType.StoredProcedure);
                if(result == null)
                {
                    _logger.LogWarning("JobOpportunity not found using stored procedure: {StoredProcedure} in JobRepository.", sp);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved a JobOpportunity by Id using stored procedure: {StoredProcedure} in JobRepository.", sp);
                }
                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving a JobOpportunity by Id using stored procedure: {StoredProcedure} in JobRepository.", sp);
                throw;
            }
        }

        public async Task AddAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Adding a new JobOpportunity using stored procedure: {StoredProcedure} in JobRepository.", sp);
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
                _logger.LogInformation("Successfully added a new JobOpportunity using stored procedure: {StoredProcedure} in JobRepository.", sp);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new JobOpportunity using stored procedure: {StoredProcedure} in JobRepository.", sp);
                throw;
            }
        }

        public async Task UpdateAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Updating a JobOpportunity using stored procedure: {StoredProcedure} in JobRepository.", sp);
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
                _logger.LogInformation("Successfully updated a JobOpportunity using stored procedure: {StoredProcedure} in JobRepository.", sp);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating a JobOpportunity using stored procedure: {StoredProcedure} in JobRepository.", sp);
                throw;
            }   
            
        }

        public async Task DeleteAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Deleting a JobOpportunity using stored procedure: {StoredProcedure} in JobRepository.", sp);
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a JobOpportunity using stored procedure: {StoredProcedure} in JobRepository.", sp);
                throw;
            }            
        }

    }
}
