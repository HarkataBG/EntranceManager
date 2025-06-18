using EntranceManager.Models;
using EntranceManager.Models.Mappers;
using EntranceManager.Services;
using EntranceManager.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EntranceManager.Controllers.Mvc
{
    [Authorize]
    public class ApartmentsMvcController : Controller
    {
        private readonly IApartmentService _apartmentService;
        private readonly IEntranceService _entranceService;
        private readonly IUsersService _usersService;

        public ApartmentsMvcController(IApartmentService apartmentService, IEntranceService entranceService, IUsersService usersService)
        {
            _apartmentService = apartmentService;
            _entranceService = entranceService;
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


        [Authorize]
        public async Task<IActionResult> Create()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var entrances = await _entranceService.GetAllEntrancesDetailsAsync(username);

            ViewBag.Entrances = entrances.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.EntranceName
            }).ToList();

            ViewBag.EntrancesData = entrances.Select(e => new
            {
                e.Id,
                e.EntranceName,
                Residents = e.Residents.Select(r => new { r.Id, r.Username }).ToList()
            }).ToList();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,EntranceManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApartmentDto dto)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (!ModelState.IsValid)
            {
                // Repopulate entrances and users for the dropdowns because the view needs them on validation errors
                var entrances = await _entranceService.GetAllEntrancesDetailsAsync(username);
                var usersWithEntrance = entrances
                    .SelectMany(e => e.Residents.Select(u => new
                    {
                        User = u,
                        EntranceId = e.Id,
                        EntranceName = e.EntranceName
                    }))
                    .ToList();

                ViewBag.Entrances = entrances.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.EntranceName
                }).ToList() ?? new List<SelectListItem>();

                ViewBag.Users = usersWithEntrance.Select(x => new SelectListItem
                {
                    Value = x.User.Id.ToString(),
                    Text = x.User.Username,
                    Group = new SelectListGroup { Name = x.EntranceId.ToString() }
                }).ToList() ?? new List<SelectListItem>(); ;

                return View(dto);
            }

            await _apartmentService.AddApartmentAsync(dto);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator,EntranceManager")]
        public async Task<IActionResult> Edit(int id)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var apt = await _apartmentService.GetApartmentByIdAsync(id);
            if (apt == null) return NotFound();

            var entrances = await _entranceService.GetAllEntrancesDetailsAsync(username);
            if (entrances == null)
            {
                ViewBag.Entrances = new List<object>();
            }
            else
            {
                ViewBag.Entrances = entrances.Select(e => new
                {
                    e.Id,
                    e.EntranceName,
                    Residents = e.Residents.Select(r => new { r.Id, r.Username }).ToList()
                }).ToList();
            }

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