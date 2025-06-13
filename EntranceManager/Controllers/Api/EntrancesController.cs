using AspNetCoreDemo.Helpers;
using EntranceManager.Exceptions;
using EntranceManager.Models;
using EntranceManager.Models.Mappers;
using EntranceManager.Services;
using EntranceManager.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntranceManager.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntrancesController : ControllerBase
    {
        private readonly IEntranceService _entranceService;

        private readonly IUsersService _usersService;

        public EntrancesController(IEntranceService entranceService, IUsersService usersService)
        {
            _entranceService = entranceService;
            _usersService = usersService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<EntranceResponseDto>> GetAccessibleEntrancesAsync()
        {          
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var allEntrances = await _entranceService.GetAllEntrancesDetailsAsync();

            var currentUser = await _usersService.GetByUsernameAsync(username);

            return currentUser.Role switch
            {
                nameof(UserRole.Administrator) => allEntrances,

                nameof(UserRole.EntranceManager) => allEntrances
                    .Where(e => currentUser.ManagedEntrances.Any(me => me.Id == e.Id)),

                _ => allEntrances
                    .Where(e => e.Residents.Any(r => r.Id == currentUser.Id))
            };
        }


        [HttpGet("{id}")]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)}")]
        public async Task<ActionResult<EntranceResponseDto>> GetEntranceById(int id)
        {
            var apartmentResponseDto = await _entranceService.GetEntranceByIdAsync(id);
            if (apartmentResponseDto == null) return NotFound();
            return Ok(apartmentResponseDto);
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)}")]
        public async Task<IActionResult> CreateEntrance([FromBody] EntranceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _entranceService.AddEntranceAsync(dto);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case EntranceAlreadyExistsException _:
                        return StatusCode(StatusCodes.Status409Conflict, ex.Message);

                    default:
                        throw;
                }
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)}")]
        public async Task<ActionResult> UpdateEntrance(int id, [FromBody] EntranceDto dto)
        {
            try
            {
                await _entranceService.UpdateEntranceAsync(id, dto);

                return NoContent();
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case EntranceNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)}")]
        public async Task<ActionResult> DeleteEntrance(int id)
        {
            try
            {
                var existingEntrance = await _entranceService.GetEntranceByIdAsync(id);
                if (existingEntrance == null)
                {
                    throw new EntranceNotFoundException(id);
                }

                await _entranceService.DeleteEntranceAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case EntranceNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }
        }

        [HttpPost("{entranceId}/users/{userId}")]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)}")]
        public async Task<IActionResult> AddUserToEntrance(int entranceId, int userId)
        {
            try
            {
                var entrance = await _entranceService.GetEntranceByIdAsync(entranceId);

                await _entranceService.AddUserToEntranceAsync(entranceId, userId);
                return Ok(new { message = "User added to entrance successfully." });
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case EntranceAlreadyExistsException _:
                        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
                    case UserNotFoundException _:
                    case EntranceNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }
        }

        [HttpDelete("{entranceId}/users/{userId}")]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)}")]
        public async Task<IActionResult> RemoveUserFromEntrance(int entranceId, int userId)
        {
            try
            { 
                var apartment = await _entranceService.GetEntranceByIdAsync(entranceId);

                await _entranceService.RemoveUserFromEntrance(entranceId, userId);
                return Ok(new { message = "User removed from entrance successfully." });
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
                    case UserNotFoundException _:
                    case ApartmentUserNotFoundException _:
                    case ApartmentNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }
        }
    }
}