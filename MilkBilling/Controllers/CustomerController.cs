using CommonLayer.CommonResponse;
using CommonLayer.PhotoUpload;
using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace MilkBilling.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IFileService _fileService;

        public CustomerController(ICustomerService customerService, IFileService fileService)
        {
            _customerService= customerService;
            _fileService= fileService;
        }

        ///Get All Customers
        [Authorize]
        [HttpGet]
        public async Task<GeneralResponse<List<CustomerListDto>>> GetCustomers()
        {
            return await _customerService.GetAllCustomersAsync();
        }

    }
}
