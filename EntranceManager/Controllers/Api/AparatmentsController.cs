﻿using AspNetCoreDemo.Helpers;
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

        public ApartmentsController(IApartmentService apartmentService, IUsersService usersService)
        {
            _apartmentService = apartmentService;
            _usersService = usersService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ApartmentResponseDto>>> GetAccessibleApartmentsAsync()
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                    throw new UnauthorizedAccessException("User is not authenticated.");

                var apartments = await _apartmentService.GetAllApartmentsDetailsAsync(username);

                return Ok(apartments);
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
        public async Task<ActionResult<ApartmentResponseDto>> GetApartmentById(int id)
        {
            var apartmentResponseDto = await _apartmentService.GetApartmentDetailsByIdAsync(id);
            if (apartmentResponseDto == null) return NotFound();
            return Ok(apartmentResponseDto);
        }
       
        [HttpPost]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)},{nameof(UserRole.EntranceManager)}")]
        public async Task<IActionResult> CreateApartment([FromBody] ApartmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _usersService.GetAuthorizedUserForEntranceAsync(User, dto.EntranceId);

                await _apartmentService.AddApartmentAsync(dto);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
                    case ApartmentAlreadyExistsException _:
                        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
                    case OwnerNotFoundException _:
                    case EntranceNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)},{nameof(UserRole.EntranceManager)}")]

        public async Task<ActionResult> UpdateApartment(int id, [FromBody] ApartmentDto dto)
        {
            try
            {
                await _usersService.GetAuthorizedUserForEntranceAsync(User, dto.EntranceId);

                await _apartmentService.UpdateApartmentAsync(id, dto);

                return NoContent();
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
                    case OwnerNotFoundException _:
                    case ApartmentNotFoundException _:
                    case EntranceNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)},{nameof(UserRole.EntranceManager)}")]

        public async Task<ActionResult> DeleteApartment(int id)
        {
            try
            {
                var existingApartment = await _apartmentService.GetApartmentByIdAsync(id);
                if (existingApartment == null)
                {
                    throw new ApartmentNotFoundException(id);
                }

                await _usersService.GetAuthorizedUserForEntranceAsync(User, existingApartment.EntranceId);

                await _apartmentService.DeleteApartmentAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
                    case ApartmentNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }
        }

        [HttpPost("{apartmentId}/users/{userId}")]
        [Authorize]
        public async Task<IActionResult> AddUserToApartment(int apartmentId, int userId)
        {
            try
            {
                var apartment = await _apartmentService.GetApartmentByIdAsync(apartmentId);

                await _usersService.GetAuthorizedUserForApartmentAsync(User, apartment);

                await _apartmentService.AddUserToApartmentAsync(apartmentId, userId);
                return Ok(new { message = "User added to apartment successfully." });
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                        return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
                    case UserAlreadyAddedException _:
                    case ApartmentAlreadyExistsException _:
                        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
                    case UserNotFoundException _:
                    case ApartmentNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);

                    default:
                        throw;
                }
            }
        }

        [HttpDelete("{apartmentId}/users/{userId}")]
        [Authorize]
        public async Task<IActionResult> RemoveUserFromApartment(int apartmentId, int userId)
        {
            try
            {
                var apartment = await _apartmentService.GetApartmentByIdAsync(apartmentId);

                await _usersService.GetAuthorizedUserForApartmentAsync(User, apartment);

                await _apartmentService.RemoveUserFromApartment(apartmentId, userId);
                return Ok(new { message = "User removed from apartment successfully." });
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