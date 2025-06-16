using EntranceManager.Models;
using EntranceManager.Models.Mappers;
using EntranceManager.Services;
using EntranceManager.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntranceManager.Controllers.Mvc
{
    [Authorize]
    public class ApartmentsMvcController : Controller
    {
        private readonly IApartmentService _apartmentService;
        private readonly IUsersService _usersService;

        public ApartmentsMvcController(IApartmentService apartmentService, IUsersService usersService)
        {
            _apartmentService = apartmentService;
            _usersService = usersService;
        }

        public async Task<IActionResult> Index()
        {
            var apartments = await _apartmentService.GetAllApartmentsDetailsAsync(User.Identity.Name);
            return View(apartments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var apartment = await _apartmentService.GetApartmentDetailsByIdAsync(id);
            if (apartment == null)
                return NotFound();

            return View(apartment);
        }

        [Authorize(Roles = "Administrator,EntranceManager")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Administrator,EntranceManager")]
        public async Task<IActionResult> Create(ApartmentDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            await _usersService.GetAuthorizedUserForEntranceAsync(User, dto.EntranceId);
            await _apartmentService.AddApartmentAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator,EntranceManager")]
        public async Task<IActionResult> Edit(int id)
        {
            var apt = await _apartmentService.GetApartmentByIdAsync(id);
            if (apt == null) return NotFound();

            return View(new ApartmentDto
            {
                EntranceId = apt.EntranceId,
                OwnerUserId = apt.OwnerUserId,
                Number = apt.Number
            });
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,EntranceManager")]
        public async Task<IActionResult> Edit(int id, ApartmentDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            await _usersService.GetAuthorizedUserForEntranceAsync(User, dto.EntranceId);
            await _apartmentService.UpdateApartmentAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator,EntranceManager")]
        public async Task<IActionResult> Delete(int id)
        {
            var apt = await _apartmentService.GetApartmentByIdAsync(id);
            if (apt == null) return NotFound();

            return View(apt);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator,EntranceManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apt = await _apartmentService.GetApartmentByIdAsync(id);
            await _usersService.GetAuthorizedUserForEntranceAsync(User, apt.EntranceId);
            await _apartmentService.DeleteApartmentAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}