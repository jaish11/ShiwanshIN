using AutoMapper;
using Dapper;
using SS.Core.DTOs;
using SS.Core.Interfaces;

namespace SS.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }
        public async Task<IEnumerable<UserDto>> GetAllUSerAsync()
        {
            var users = await _userRepo.GetAllAsync("sp_GetAllUsers");
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        public async Task<UserDto> GetByIdAsync(int id)
        {
            var p = new DynamicParameters();
            p.Add("@Id", id);
            var user = await _userRepo.GetByIdAsync("sp_GetUserById", p);
            return _mapper.Map<UserDto>(user);
        }
        public async Task UpdateUserAsync(int id, string fullName, bool isActive)
        {
            var p = new DynamicParameters();
            p.Add("@Id", id);
            p.Add("@FullName", fullName);
            p.Add("@IsActive", isActive);
            await _userRepo.UpdateAsync("sp_UpdateUser", p);
        }
        public async Task UpdateRoleAsync(int id, string role)
        {
            var p = new DynamicParameters();
            p.Add("@Id", id);
            p.Add("@Role", role);
            await _userRepo.UpdateRoleAsync("sp_UpdateUserRole", p);
        }
        public async Task DeleteUserAsync(int id)
        {
            var p = new DynamicParameters();
            p.Add("@Id", id);
            await _userRepo.DeleteAsync("sp_DeleteUser", p);
        }
    }
}
