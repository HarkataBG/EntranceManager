using EntranceManager.Models;
using EntranceManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace EntranceManager.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntrancesController : ControllerBase
    {
        private readonly IEntranceService _entranceService;

        public EntrancesController(IEntranceService entranceService)
        {
            _entranceService = entranceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entrance>>> GetAll()
        {
            var entrances = await _entranceService.GetAllEntrancesAsync();
            return Ok(entrances);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Entrance>> GetById(int id)
        {
            var entrance = await _entranceService.GetEntranceByIdAsync(id);
            if (entrance == null) return NotFound();
            return Ok(entrance);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Entrance entrance)
        {
            await _entranceService.AddEntranceAsync(entrance);
            return CreatedAtAction(nameof(GetById), new { id = entrance.Id }, entrance);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Entrance entrance)
        {
            if (id != entrance.Id) return BadRequest();

            var existing = await _entranceService.GetEntranceByIdAsync(id);
            if (existing == null) return NotFound();

            await _entranceService.UpdateEntranceAsync(entrance);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await _entranceService.GetEntranceByIdAsync(id);
            if (existing == null) return NotFound();

            await _entranceService.DeleteEntranceAsync(id);
            return NoContent();
        }
    }
}