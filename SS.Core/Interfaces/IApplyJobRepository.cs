
using Dapper;
using SS.Core.Entities;

namespace SS.Core.Interfaces
{
    public interface IApplyJobRepository
    {
        Task<IEnumerable<ApplyJob>> GetAllAsync(string sp);
        Task<ApplyJob> GetByIdAsync(string sp, DynamicParameters parameters);
        Task AddAsync(string sp, DynamicParameters parameters);
        Task UpdateAsync(string sp, DynamicParameters parameters);
        Task DeleteAsync(string sp, DynamicParameters parameters);

    }
}
