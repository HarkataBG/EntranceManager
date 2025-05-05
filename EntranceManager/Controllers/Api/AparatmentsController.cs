using AspNetCoreDemo.Helpers;
using EntranceManager.Models;
using EntranceManager.Models.Mappers;
using EntranceManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace EntranceManager.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApartmentsController : ControllerBase
    {
        private readonly IApartmentService _apartmentService;

        private readonly ModelMapper _modelMapper;

        public ApartmentsController(IApartmentService apartmentService, ModelMapper modelMapper)
        {
            _apartmentService = apartmentService;
            _modelMapper = modelMapper;
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
        public async Task<IActionResult> CreateApartment([FromBody] ApartmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Apartment apartment = _modelMapper.Map(dto);
            await _apartmentService.AddApartmentAsync(apartment);

            return Ok(apartment);
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