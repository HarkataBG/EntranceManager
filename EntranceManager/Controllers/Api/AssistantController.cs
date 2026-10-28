using EntranceManager.Models.AI;
using EntranceManager.Services.AI;
using EntranceManager.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntranceManager.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssistantController : ControllerBase
    {
        private readonly IEntranceFinancialService _financialService;
        private readonly IAIService _aiService;

        public AssistantController(
            IEntranceFinancialService financialService,
            IAIService aiService)
        {
            _financialService = financialService;
            _aiService = aiService;
        }

        [HttpGet("financial-summary")]
        public async Task<ActionResult<
            IReadOnlyList<FinancialAssistantResponseDto>>> GetFinancialSummary(
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

            var responses = new List<FinancialAssistantResponseDto>();

            foreach (var summary in summaries)
            {
                var explanation =
                    await _aiService.GenerateFinancialSummaryAsync(
                        summary,
                        cancellationToken);

                responses.Add(new FinancialAssistantResponseDto
                {
                    FinancialSummary = summary,
                    Explanation = explanation
                });
            }

            return Ok(responses);
        }
    }
}