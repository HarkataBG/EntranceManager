namespace EntranceManager.Controllers.View
{
    using global::EntranceManager.Models.Mappers;
    using global::EntranceManager.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace EntranceManager.Controllers.MVC
    {
        [Authorize]
        public class EntrancesMvcController : Controller
        {
            private readonly IEntranceService _entranceService;

            public EntrancesMvcController(IEntranceService entranceService)
            {
                _entranceService = entranceService;
            }

            public async Task<IActionResult> Index()
            {
                var username = User.Identity?.Name;
                var entrances = await _entranceService.GetAllEntrancesDetailsAsync(username);
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
                    Address = entrance.Address
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

            [Authorize(Roles = "Administrator")]
            public async Task<IActionResult> Delete(int id)
            {
                var entrance = await _entranceService.GetEntranceByIdAsync(id);
                if (entrance == null) return NotFound();

                return View(entrance);
            }

            [HttpPost, ActionName("Delete")]
            [Authorize(Roles = "Administrator")]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                await _entranceService.DeleteEntranceAsync(id);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
