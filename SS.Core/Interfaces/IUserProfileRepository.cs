
using Dapper;
using SS.Core.Entities;

namespace SS.Core.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<IEnumerable<UserProfile>> GetAllAsync(string sp);
        Task<UserProfile> GetByIdAsync(string sp, DynamicParameters parameters);
        Task<UserProfile> GetByUserIdAsync(string sp, DynamicParameters parameters);
        Task AddAsync(string sp, DynamicParameters parameters);
        Task UpdateAsync(string sp, DynamicParameters parameters);
        Task DeleteAsync(string sp, DynamicParameters parameters);
    }
}
