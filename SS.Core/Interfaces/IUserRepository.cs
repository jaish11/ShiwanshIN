
using Dapper;
using SS.Core.Entities;

namespace SS.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<int> AddAsync(string sp, DynamicParameters parameters);
        Task<User> GetByEmailAsync(string sp, DynamicParameters parameters);
        Task<User> GetByIdAsync(string sp, DynamicParameters parameters);
        Task<IEnumerable<User>> GetAllAsync(string sp);
        Task UpdateAsync(string sp, DynamicParameters parameters);
        Task DeleteAsync(string sp, DynamicParameters parameters);
        Task UpdateRoleAsync(string sp, DynamicParameters parameters);
        Task<User> GetByEmailTokenAsync(string sp, DynamicParameters parameters);
        Task MarkEmailVerifiedAsync(string sp, DynamicParameters parameters);
        Task UpdateEmailTokenAsync(string sp, DynamicParameters parameters);
        //Reset/Forgot Password
        Task SetPasswordResetTokenAsync(string sp, DynamicParameters parameters);
        Task<User> GetByResetTokenAsync(string sp, DynamicParameters parameters);
        Task UpdatePasswordAsync(string sp, DynamicParameters parameters);

    }
}
