using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SS.Core.Entities;
using SS.Core.Interfaces;
using System.Data;

namespace SS.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public JobRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlCon");
        }

        public async Task<IEnumerable<JobOpportunity>> GetAllAsync(string sp)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<JobOpportunity>(sp, commandType: CommandType.StoredProcedure);
        }

        public async Task<JobOpportunity> GetByIdAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<JobOpportunity>(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task AddAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(string sp, DynamicParameters parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
        }


        //public async Task AddAsync(string sp, DynamicParameters parameters)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
        //    }
        //}

        //public async Task DeleteAsync(string sp, DynamicParameters parameters)
        //{
        //    await using (var connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
        //    }
        //}

        //public Task<IEnumerable<JobOpportunity>> GetAllAsync(string sp)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        var result = connection.QueryAsync<JobOpportunity>(sp, commandType: CommandType.StoredProcedure);
        //        return result;
        //    }
        //}

        //public Task<JobOpportunity> GetByIdAsync(string sp, DynamicParameters parameters)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        var result = connection.QueryFirstOrDefaultAsync<JobOpportunity>(sp, parameters, commandType: CommandType.StoredProcedure);
        //        return result;
        //    }
        //}

        //public Task UpdateAsync(string sp, DynamicParameters parameters)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        var result = connection.ExecuteAsync(sp, parameters, commandType: CommandType.StoredProcedure);
        //        return result;
        //    }
        //}

    }
}
