using EntranceManager.Exceptions;
using EntranceManager.Exceptions.EntranceManager.Exceptions;
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
        public async Task<ActionResult<IEnumerable<FeeResponseDto>>> GetAllFees()
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                    throw new UnauthorizedAccessException("User is not authenticated.");

                var allFees = await _feesService.GetAllFeeDetailsAsync(username);

                return Ok(allFees);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
                    case OwnerNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }

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
                    case FeeAlreadyExistsException _:
                        return StatusCode(StatusCodes.Status409Conflict, ex.Message);

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