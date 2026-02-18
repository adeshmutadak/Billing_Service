using CommonLayer.CommonResponse;
using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Service;

namespace MilkBilling.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class MilkEntries : ControllerBase
    {
        private readonly IMilkService _milkService;
        private readonly ICustomerRepo _customerRepo;
        public MilkEntries(IMilkService milkService ,ICustomerRepo customerRepo)
        {
            _milkService = milkService;
            _customerRepo = customerRepo;
        }

        [Authorize]
        [HttpPost]
        public async Task<GeneralResponse<AddMilkEntryDto>> AddMilkEntry(AddMilkEntryDto dto)
        {
            return await _milkService.AddMilkEntryAsync(dto);
        }


        [Authorize]
        [HttpGet("getmilkOnId")]
        public async Task<IActionResult> GetMilkEntriesByCustomer(int customerId, int userId)
        {
            var response = await _milkService.GetMilkEntriesByCustomerAndUserAsync(customerId, userId);
            return StatusCode((int)response.HttpStatusCode, response);
        }

        [Authorize]
        [HttpPut("updateMilkEntry")]
        public async Task<IActionResult> UpdateMilkEntry( 
     [FromBody] AddMilkEntryDto model)
        {
            var response = await _milkService.UpdateMilkEntryAsync( model);
            return StatusCode((int)response.HttpStatusCode, response);
        }


        [Authorize]
        [HttpDelete("deleteMilkEntry/{entryId}")]
        public async Task<IActionResult> DeleteMilkEntry(int entryId)
        {
            var response = await _milkService.DeleteMilkEntryAsync(entryId);
            return StatusCode((int)response.HttpStatusCode, response);
        }


    }
}
