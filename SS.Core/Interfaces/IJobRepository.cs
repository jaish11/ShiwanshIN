
using Dapper;
using SS.Core.Entities;

namespace SS.Core.Interfaces
{
    public interface IJobRepository
    {
        Task<IEnumerable<JobOpportunity>> GetAllAsync(string sp);
        Task<JobOpportunity> GetByIdAsync(string sp, DynamicParameters parameters);
        Task AddAsync(string sp, DynamicParameters parameters);
        Task UpdateAsync(string sp, DynamicParameters parameters);
        Task DeleteAsync(string sp, DynamicParameters parameters);
    }
}
