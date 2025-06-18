using EntranceManager.Exceptions;
using EntranceManager.Models;
using EntranceManager.Models.Mappers;
using EntranceManager.Services;
using EntranceManager.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EntranceManager.Controllers
{
    [Authorize]
    public class FeesMvcController : Controller
    {
        private readonly IFeesService _feesService;
        private readonly IEntranceService _entranceService;
        private readonly IUsersService _usersService;

        public FeesMvcController(IFeesService feesService, IEntranceService entranceService, IUsersService usersService)
        {
            _feesService = feesService;
            _entranceService = entranceService;
            _usersService = usersService;
        }

        public async Task<IActionResult> Index()
        {
            var username = User.Identity?.Name;
            var fees = await _feesService.GetAllFeeDetailsAsync(username);
            return View(fees);
        }

        public async Task<IActionResult> Details(int id)
        {
            var fee = await _feesService.GetFeeDetailsByIdAsync(id);
            return View(fee);
        }

        [Authorize(Roles = "Administrator,EntranceManager")]
        public async Task<IActionResult> Create()
        {
            var username = User.Identity?.Name;
            var entrances = await _entranceService.GetAllEntrancesDetailsAsync(username);
            ViewBag.Entrances = new SelectList(entrances, "Id", "EntranceName");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,EntranceManager")]
        public async Task<IActionResult> Create(FeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                var username = User.Identity?.Name;
                var entrances = await _entranceService.GetAllEntrancesDetailsAsync(username);
                ViewBag.Entrances = new SelectList(entrances, "Id", "EntranceName");
                return View(dto);
            }

            await _usersService.GetAuthorizedUserForEntranceAsync(User, dto.EntranceId);
            await _feesService.CreateFeeAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var fee = await _feesService.GetFeeDetailsByIdAsync(id);
            if (fee == null) return NotFound();

            await PopulateEntrancesDropdownAsync(fee.Entrance.Id);
            return View(fee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FeeResponseDto model)
        {           

            try
            {
                var entrance = await _entranceService.GetEntranceByIdAsync(model.Entrance.Id);

                var feeDto = new FeeDto
                {
                    Name = model.Name,
                    Description = model.Description,
                    Amount = model.Amount,
                    EntranceId = model.Entrance.Id,
                };

                await _feesService.UpdateFeeAsync(model.Id, feeDto);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Грешка: {ex.Message}");
            }

            await PopulateEntrancesDropdownAsync(model.Entrance.Id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _feesService.DeleteFeeAsync(id);
                TempData["SuccessMessage"] = "Таксата беше изтрита успешно.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Грешка при изтриване: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


        private async Task PopulateEntrancesDropdownAsync(int? selectedEntranceId = null)
        {
            var username = User.Identity?.Name;
            var entrances = await _entranceService.GetAllEntrancesDetailsAsync(username);
            ViewBag.Entrances = entrances.Select(e =>
                new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.EntranceName,
                    Selected = (selectedEntranceId.HasValue && e.Id == selectedEntranceId.Value)
                }).ToList();
        }
    }
}