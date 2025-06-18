using EntranceManager.Models.Mappers;
using EntranceManager.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EntranceManager.Controllers
{
    public class PaymentsMvcController : Controller
    {
        private readonly IPaymentsService _paymentService;

        public PaymentsMvcController(IPaymentsService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public IActionResult Pay(int apartmentId, int feeId, decimal amount)
        {
            var model = new PaymentDto
            {
                ApartmentId = apartmentId,
                FeeId = feeId,
                Amount = amount
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(PaymentDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Set payment method, e.g., from form or default
            model.PaymentMethod ??= "MockedCreditCard";

            try
            {
                await _paymentService.ProcessPaymentAsync(model);
                TempData["SuccessMessage"] = "Плащането беше успешно!";
                return RedirectToAction("Index", "ApartmentsMvc", new { id = model.ApartmentId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Грешка при плащането: {ex.Message}");
                return View(model);
            }
        }
    }
}