using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace MilkBilling.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillService _billService;

        public BillController(IBillService billService)
        {
            _billService = billService;
        }

        // Taken from the JWT rather than the request body, so a caller cannot
        // generate a bill against another user's customer.
        private int CurrentUserId => int.Parse(User.FindFirst("UserId")!.Value);

        /// <summary>
        /// Generates the monthly bill for one customer.
        /// TotalCowLitre, TotalBuffaloLitre and TotalAmount are calculated from
        /// milkentries for the given Month and Year; PreviousBalance is supplied
        /// by the caller and TotalPayable is their sum.
        /// Re-posting the same period recalculates the existing bill, unless it
        /// is already marked paid, which returns 409.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddBill([FromBody] AddBillRequestDto dto)
        {
            var response = await _billService.AddBillAsync(CurrentUserId, dto);
            return StatusCode((int)response.HttpStatusCode, response);
        }



        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] BillHistoryFilterDto filter)
        {
            var response = await _billService.GetHistoryAsync(CurrentUserId, filter);
            return StatusCode((int)response.HttpStatusCode, response);
        }
    }
}


