using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace MilkBilling.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        // Taken from the JWT rather than the query string, so changing a URL
        // parameter cannot expose another user's customers.
        private int CurrentUserId => int.Parse(User.FindFirst("UserId")!.Value);

        /// <summary>
        /// History with filters. Examples:
        ///   ?year=2025&amp;includeDetail=true
        ///       customers created in 2025, all twelve months with day detail
        ///   ?year=2025&amp;includeAllCustomers=true&amp;includeDetail=true
        ///       every customer regardless of when they were created
        ///   ?year=2025&amp;customerId=1&amp;month=12&amp;includeEmptyDays=true
        ///       one customer, one month, every day of the month
        ///   ?year=2025&amp;isPaymentDone=false
        ///       months with no settled payment
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetHistory([FromQuery] HistoryFilterDto filter)
        {
            var response = await _historyService.GetHistoryAsync(CurrentUserId, filter);
            return StatusCode((int)response.HttpStatusCode, response);
        }

        /// <summary>Years and customers available, for the filter dropdowns.
        /// Years cover milk entries, bills, payments and customer creation.</summary>
        [Authorize]
        [HttpGet("filters")]
        public async Task<IActionResult> GetFilterOptions()
        {
            var response = await _historyService.GetFilterOptionsAsync(CurrentUserId);
            return StatusCode((int)response.HttpStatusCode, response);
        }
    }
}
