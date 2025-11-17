
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SS.Core.Entities;
using SS.Core.Interfaces;
using System.Data;

namespace SS.Infrastructure.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly ILogger<UserProfileRepository> _logger;

        public UserProfileRepository(IConfiguration configuration, ILogger<UserProfileRepository> logger)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlCon");
            _logger = logger;
        }

        public async Task AddAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Adding user profile with SP: {sp}", sp);
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);

        }

        public async Task DeleteAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Deleting user profile with SP: {sp}", sp);
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);

        }

        public async Task<IEnumerable<UserProfile>> GetAllAsync(string sp)
        {
            _logger.LogInformation("Retrieving all user profiles using SP: {sp}", sp);
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<UserProfile>(sp, commandType: CommandType.StoredProcedure);
        }

        public async Task<UserProfile> GetByIdAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Retrieving user profile by id with SP: {sp}", sp);
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<UserProfile>(sp, parameters, commandType: CommandType.StoredProcedure);

        }

        public async Task<UserProfile> GetByUserIdAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Retrieving user profile by userId with SP: {sp}", sp);
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<UserProfile>(sp, parameters, commandType: CommandType.StoredProcedure);

        }

        public async Task UpdateAsync(string sp, DynamicParameters parameters)
        {
            _logger.LogInformation("Updating user profile with SP: {sp}", sp);
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);

        }
    }
}
