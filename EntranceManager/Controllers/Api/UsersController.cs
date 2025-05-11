using EntranceManager.Models.Mappers;
using EntranceManager.Services;
using EntranceManager.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntranceManager.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("promote")]
        public async Task<IActionResult> PromoteEntranceManager([FromBody] EntranceManagerDto dto)
        {
            await _usersService.PromoteToManagerAsync(dto.UserId, dto.EntranceId);
            return Ok("User promoted to Entrance Manager successfully.");
        }

    }
}
