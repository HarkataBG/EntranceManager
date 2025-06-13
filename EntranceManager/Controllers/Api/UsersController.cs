using EntranceManager.Exceptions;
using EntranceManager.Models;
using EntranceManager.Models.Mappers;
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

        [Authorize(Roles = nameof(UserRole.Administrator))]
        [HttpPost("promote")]
        public async Task<IActionResult> PromoteEntranceManager([FromBody] EntranceManagerDto dto)
        {
            try
            {
                await _usersService.PromoteToManagerAsync(dto.UserId, dto.EntranceId);
                return Ok("User promoted to Entrance Manager successfully.");
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case EntranceNotFoundException _:
                    case UserNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }
        }
        [Authorize(Roles = nameof(UserRole.Administrator))]
        [HttpPost("demote/{entranceId}")]
        public async Task<IActionResult> DemoteEntranceManager(int entranceId)
        {
            try
            {
                await _usersService.DemoteFromManagerAsync(entranceId);
                return Ok("User demoted from Entrance Manager successfully.");
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case EntranceNotFoundException _:
                    case ManagerNotFoundException _:
                    case UserNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }
        }

    }
}
