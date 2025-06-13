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
    public class FeesController : ControllerBase
    {
        private readonly IFeesService _feesService;
        private readonly IUsersService _usersService;

        public FeesController(IFeesService feesService, IUsersService usersService)
        {
            _feesService = feesService;
            _usersService = usersService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<FeeResponseDto>> GetAllFees()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var allFees = await _feesService.GetAllFeeDetailsAsync();

            var currentUser = await _usersService.GetByUsernameAsync(username);

            return currentUser.Role switch
            {
                nameof(UserRole.Administrator) => allFees,

                nameof(UserRole.EntranceManager) => allFees
                    .Where(e => currentUser.ManagedEntrances.Any(me => me.Id == e.Id)),

                _ => new List<FeeResponseDto>()
            };

        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)}")]
        public async Task<ActionResult<FeeResponseDto>> GetFeeById(int id)
        {
            var fee = await _feesService.GetFeeDetailsByIdAsync(id);

            return Ok(fee);
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)},{nameof(UserRole.EntranceManager)}")]
        public async Task<IActionResult> CreateFee([FromBody] FeeDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _usersService.GetAuthorizedUserForEntranceAsync(User, dto.EntranceId);

                await _feesService.CreateFeeAsync(dto);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
                    case EntranceNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)},{nameof(UserRole.EntranceManager)}")]
        public async Task<IActionResult> UpdateFee(int id, [FromBody] FeeDto dto)
        {
            try
            {
                await _usersService.GetAuthorizedUserForEntranceAsync(User, dto.EntranceId);

                await _feesService.UpdateFeeAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
                    case FeeNotFoundException _:
                    case EntranceNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message); 
                    default:
                        throw;
                }
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)},{nameof(UserRole.EntranceManager)}")]
        public async Task<IActionResult> DeleteFee(int id)
        {
            try
            {
                var fee = await _feesService.GetByIdAsync(id);

                await _usersService.GetAuthorizedUserForEntranceAsync(User, fee.EntranceId);

                await _feesService.DeleteFeeAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
                    case FeeNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);
                    default:
                        throw;
                }
            }
        }
    }
}