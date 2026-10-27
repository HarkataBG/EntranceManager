using EntranceManager.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntranceManager.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FinancialController : ControllerBase
    {
        private readonly IEntranceFinancialService _financialService;

        public FinancialController(IEntranceFinancialService financialService)
        {
            _financialService = financialService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(
            CancellationToken cancellationToken)
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(username))
            {
                return Unauthorized();
            }

            var summaries =
                await _financialService.GetAccessibleFinancialSummaryAsync(
                    username,
                    cancellationToken);

            return Ok(summaries);
        }
    }
}
