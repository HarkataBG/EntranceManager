using AspNetCoreDemo.Helpers;
using EntranceManager.Exceptions;
using EntranceManager.Models;
using EntranceManager.Models.Mappers;
using EntranceManager.Services;
using EntranceManager.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EntranceManager.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApartmentsController : ControllerBase
    {
        private readonly IApartmentService _apartmentService;

        private readonly IUsersService _usersService;

        private readonly ModelMapper _modelMapper;

        public ApartmentsController(IApartmentService apartmentService, IUsersService usersService, ModelMapper modelMapper)
        {
            _apartmentService = apartmentService;
            _usersService = usersService;
            _modelMapper = modelMapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Apartment>> GetAccessibleApartmentsAsync()
        {
            var allApartments = await _apartmentService.GetAllApartmentsAsync();

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var currentUser = await _usersService.GetByUsernameAsync(username);

            return currentUser.Role switch
            {
                "Administrator" => allApartments,

                "EntranceManager" => allApartments
                    .Where(a => currentUser.ManagedEntrances.Any(e => e.Id == a.EntranceId)),

                _ => allApartments
                    .Where(a =>
                        a.OwnerUserId == currentUser.Id ||
                        a.ApartmentUsers.Any(au => au.UserId == currentUser.Id))
            };
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Apartment>> GetApartmentById(int id)
        {
            var apartment = await _apartmentService.GetApartmentByIdAsync(id);
            if (apartment == null) return NotFound();
            return Ok(apartment);
        }
       
        [HttpPost]
        [Authorize(Roles = "EntranceManager,Administrator")]
        public async Task<IActionResult> CreateApartment([FromBody] ApartmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var apartment = _modelMapper.Map(dto);

                await _usersService.GetAuthorizedUserForEntranceAsync(User, apartment.EntranceId);

                await _apartmentService.AddApartmentAsync(apartment);

                return Ok(apartment);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case OwnerNotFoundException _:
                    case EntranceNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "EntranceManager,Administrator")]
        public async Task<ActionResult> UpdateApartment(int id, Apartment apartment)
        {
            try
            {
                if (id != apartment.Id) return BadRequest();

                var existing = await _apartmentService.GetApartmentByIdAsync(id);
                if (existing == null) return NotFound();

                await _usersService.GetAuthorizedUserForEntranceAsync(User, apartment.EntranceId);

                await _apartmentService.UpdateApartmentAsync(apartment);

                return NoContent();
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case OwnerNotFoundException _:
                    case EntranceNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "EntranceManager,Administrator")]
        public async Task<ActionResult> DeleteApartment(int id)
        {
            var existingApartment = await _apartmentService.GetApartmentByIdAsync(id);
            if (existingApartment == null) return NotFound();

            await _usersService.GetAuthorizedUserForEntranceAsync(User, existingApartment.EntranceId);

            await _apartmentService.DeleteApartmentAsync(id);
            return NoContent();
        }

        [HttpPost("{apartmentId}/users/{userId}")]
        [Authorize]
        public async Task<IActionResult> AddUserToApartment(int apartmentId, int userId)
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                    return Unauthorized();

                var currentUser = await _usersService.GetByUsernameAsync(username);
                var apartment = await _apartmentService.GetApartmentByIdAsync(apartmentId);

                if (apartment == null)
                    return NotFound("Apartment not found.");

                if (currentUser.Role != "Administrator")
                { 
                    if (currentUser.Role == "EntranceManager" &&
                        !currentUser.ManagedEntrances.Any(e => e.Id == apartment.EntranceId))
                    {
                        return Forbid("You are not allowed to modify apartments outside your managed entrances.");
                    }

                    if (currentUser.Role == "User" &&
                        apartment.OwnerUserId != currentUser.Id)
                    {
                        return Forbid("You are not allowed to modify apartments you don't own.");
                    }
                }

                await _apartmentService.AddUserToApartmentAsync(apartmentId, userId);
                return Ok(new { message = "User added to apartment successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unexpected error occurred.", detail = ex.Message });
            }
        }
    }
}