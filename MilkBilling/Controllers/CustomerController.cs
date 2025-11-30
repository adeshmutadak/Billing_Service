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

        /// <summary>
        /// Get All Customer
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<GeneralResponse<List<CustomerListDto>>> GetCustomers()
        {
            int userId = int.Parse(User.FindFirst("UserId")!.Value);
            return await _customerService.GetAllCustomersAsync(userId);
        }



        /// <summary>
        /// Add Customer 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<GeneralResponse<CustomerListDto>> AddCustomer(AddCustomerRequestDto dto)
        {
            return await _customerService.AddCustomerAsync(dto);
        }

        /// <summary>
        /// Update Customer
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public async Task<GeneralResponse<CustomerListDto>> UpdateCustomer(UpdateCustomerRequestDto dto)
        {
            return await _customerService.UpdateCustomerAsync(dto);
        }

        /// <summary>
        /// Get customer based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<GeneralResponse<CustomerListDto>> GetCustomerById(int id)
        {
            return await _customerService.GetCustomerByIdAsync(id);
        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<GeneralResponse<string>> DeleteCustomer(int id)
        {
            return await _customerService.DeleteCustomerAsync(id);
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchCustomers(string? name = null, string? phoneNumber = null,string? address = null)
        {
            var response = await _customerService.SearchCustomersAsync(name, phoneNumber, address);
            return StatusCode((int)response.HttpStatusCode, response);
        }

    }
}
