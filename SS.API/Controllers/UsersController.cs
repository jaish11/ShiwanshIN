using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.Application.Services;
using SS.Core.DTOs;

namespace SS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;
        public UsersController(UserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.GetAllUSerAsync());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _userService.GetByIdAsync(id));
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto dto)
        {
            var allowedRoles = new[] { "User", "Admin", "SuperAdmin" };

            // If no role is given → default to "User"
            var role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role;

            if (!allowedRoles.Contains(role))
                return BadRequest("Invalid role");

            var id = await _authService.RegisterAsync(dto, role);

            return Ok(new { message = "User created successfully", id });
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserDto dto)
        {
            await _userService.UpdateUserAsync(dto.Id, dto.FullName, dto.IsActive);
            return Ok(new { message = "User updated" });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateById(int id, [FromBody] UserDto dto)
        {
            await _userService.UpdateUserAsync(id, dto.FullName, dto.IsActive);
            return Ok(new { message = "User updated" });
        }
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRoleById([FromBody] UserDto dto)
        {
            // validate role string
            var allowed = new[] { "User", "Admin", "SuperAdmin" };
            if (!allowed.Contains(dto.Role)) return BadRequest("Invalid role");
            await _userService.UpdateRoleAsync(dto.Id, dto.Role);
            return Ok(new { message = "Role updated" });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new { message = "User Delete" });
        }
    }
}
