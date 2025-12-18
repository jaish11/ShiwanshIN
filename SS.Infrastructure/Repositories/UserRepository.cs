using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SS.Core.Entities;
using SS.Core.Interfaces;
using System.Data;

namespace SS.Infrastructure.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlCon");
        }
        public async Task<int> AddAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<User>> GetAllAsync(string sp)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<User>(sp, commandType: CommandType.StoredProcedure);
        }

        public async Task<User> GetByEmailAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<User> GetByIdAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateRoleAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<User> GetByEmailTokenAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task MarkEmailVerifiedAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task UpdateEmailTokenAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task SetPasswordResetTokenAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<User> GetByResetTokenAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdatePasswordAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
        }

    }
}
