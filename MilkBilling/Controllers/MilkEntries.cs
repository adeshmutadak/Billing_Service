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

    }
}
