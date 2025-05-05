using EntranceManager.Models;
using EntranceManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace EntranceManager.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApartmentsController : ControllerBase
    {
        private readonly IApartmentService _apartmentService;

        public ApartmentsController(IApartmentService apartmentService)
        {
            _apartmentService = apartmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Apartment>>> GetAll()
        {
            var apartments = await _apartmentService.GetAllApartmentsAsync();
            return Ok(apartments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Apartment>> GetById(int id)
        {
            var apartment = await _apartmentService.GetApartmentByIdAsync(id);
            if (apartment == null) return NotFound();
            return Ok(apartment);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Apartment apartment)
        {
            await _apartmentService.AddApartmentAsync(apartment);
            return CreatedAtAction(nameof(GetById), new { id = apartment.Id }, apartment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Apartment apartment)
        {
            if (id != apartment.Id) return BadRequest();

            var existing = await _apartmentService.GetApartmentByIdAsync(id);
            if (existing == null) return NotFound();

            await _apartmentService.UpdateApartmentAsync(apartment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await _apartmentService.GetApartmentByIdAsync(id);
            if (existing == null) return NotFound();

            await _apartmentService.DeleteApartmentAsync(id);
            return NoContent();
        }
    }
}