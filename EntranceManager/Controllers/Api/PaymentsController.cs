using EntranceManager.Data;
using EntranceManager.Exceptions;
using EntranceManager.Models;
using EntranceManager.Models.Mappers;
using EntranceManager.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EntranceManager.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentsService _paymentService;

        public PaymentsController(IPaymentsService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> PayAsync([FromBody] PaymentDto request)
        {
            try
            {
                await _paymentService.ProcessPaymentAsync(request);

                return Ok();
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ApartmentNotFoundException _:
                        return StatusCode(StatusCodes.Status404NotFound, ex.Message);
                    case InvalidOperationException:
                        return StatusCode(StatusCodes.Status409Conflict, ex.Message);

                    default:
                        throw;
                }
            }
        }
    }
}
