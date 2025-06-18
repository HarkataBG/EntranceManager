namespace EntranceManager.Controllers.View
{
    using global::EntranceManager.Models.Mappers;
    using global::EntranceManager.Services;
    using global::EntranceManager.Services.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace EntranceManager.Controllers.MVC
    {
        [Authorize]
        public class EntrancesMvcController : Controller
        {
            private readonly IEntranceService _entranceService;
            private readonly IUsersService _usersService;

            public EntrancesMvcController(IEntranceService entranceService, IUsersService usersService)
            {
                _entranceService = entranceService;
                _usersService = usersService;
            }

            public async Task<IActionResult> Index()
            {
                var username = User.Identity?.Name;
                var entrances = await _entranceService.GetAllEntrancesDetailsAsync(username);

                var users = await _usersService.GetAllUsersAsync();

                ViewBag.Users = users;

                return View(entrances);
            }

            [Authorize(Roles = "Administrator")]
            public IActionResult Create()
            {

                return View();
            }

            [HttpPost]
            [Authorize(Roles = "Administrator")]
            public async Task<IActionResult> Create(EntranceDto dto)
            {
                if (!ModelState.IsValid) return View(dto);

                await _entranceService.AddEntranceAsync(dto);
                return RedirectToAction(nameof(Index));
            }

            [Authorize(Roles = "Administrator")]
            public async Task<IActionResult> Edit(int id)
            {
                var entrance = await _entranceService.GetEntranceByIdAsync(id);
                if (entrance == null) return NotFound();

                var dto = new EntranceDto
                {
                    EntranceName = entrance.EntranceName,
                    Address = entrance.Address,
                    City = entrance.City,
                    PostCode = entrance.PostCode,
                    CountChildrenAsResidents = entrance.CountChildrenAsResidents
                };

                return View(dto);
            }

            [HttpPost]
            [Authorize(Roles = "Administrator")]
            public async Task<IActionResult> Edit(int id, EntranceDto dto)
            {
                if (!ModelState.IsValid) return View(dto);

                await _entranceService.UpdateEntranceAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }

            [HttpPost]
            [Authorize(Roles = "Administrator")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete(int id)
            {
                await _entranceService.DeleteEntranceAsync(id);
                return RedirectToAction(nameof(Index));
            }

            [HttpPost]
            [Authorize(Roles = "Administrator")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> AddUser(int entranceId, int userId)
            {
                try
                {
                    var entrance = await _entranceService.GetEntranceByIdAsync(entranceId);
                    if (entrance == null) return NotFound();

                    await _entranceService.AddUserToEntranceAsync(entranceId, userId);
                    TempData["SuccessMessage"] = "Потребителят беше добавен успешно.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }

                return RedirectToAction(nameof(Details), new { id = entranceId });
            }

            [HttpPost]
            [Authorize(Roles = "Administrator")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> RemoveUser(int entranceId, int userId)
            {
                try
                {
                    var entrance = await _entranceService.GetEntranceByIdAsync(entranceId);
                    if (entrance == null) return NotFound();

                    await _entranceService.RemoveUserFromEntrance(entranceId, userId);
                    TempData["SuccessMessage"] = "Потребителят беше премахнат успешно.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }

                return RedirectToAction(nameof(Details), new { id = entranceId });
            }

            public async Task<IActionResult> Details(int id)
            {
                var entrance = await _entranceService.GetEntranceDetailsByIdAsync(id);
                if (entrance == null) return NotFound();

                // Defensive check: flatten if nested list
                IEnumerable<ResidentDto> residents;
                if (entrance.Residents is IEnumerable<IEnumerable<ResidentDto>> nested)
                {
                    residents = nested.SelectMany(x => x);
                }
                else
                {
                    residents = entrance.Residents;
                }

                var assignedUserIds = residents.Select(r => r.Id).ToList();

                var allUsers = await _usersService.GetAllUsersAsync();
                var availableUsers = allUsers.Where(u => !assignedUserIds.Contains(u.Id)).ToList();

                ViewBag.AvailableUsers = availableUsers;

                return View(entrance);
            }

            [HttpPost]
            [Authorize(Roles = "Administrator")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> PromoteManager(int EntranceId, int UserId)
            {
                try
                {
                    await _usersService.PromoteToManagerAsync(UserId, EntranceId);
                    TempData["SuccessMessage"] = "Потребителят беше назначен за домоуправител успешно.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
                return RedirectToAction(nameof(Index));
            }

            [HttpPost]
            [Authorize(Roles = "Administrator")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DemoteManager(int entranceId)
            {
                try
                {
                    await _usersService.DemoteFromManagerAsync(entranceId);
                    TempData["SuccessMessage"] = "Домоуправителят беше премахнат успешно.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
