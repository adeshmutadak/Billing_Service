using CommonLayer.CommonResponse;
using CommonLayer.PhotoUpload;
using Dto.Request;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CustomerService : ICustomerService 
    {

        private readonly ICustomerRepo _customerRepo;
        private readonly IFileService _fileService;

       public CustomerService (ICustomerRepo customerRepo, IFileService fileService)
        {
            _customerRepo= customerRepo;
            _fileService= fileService;
        }

        public async Task<GeneralResponse<List<CustomerListDto>>> GetAllCustomersAsync()
        {
            var customers = await _customerRepo.GetAllCustomersAsync();

            var result = customers.Select(c => new CustomerListDto
            {
                CustomerId = c.CustomerId,
                Name = c.Name,
                Address = c.Address,
                PhotoUrl = c.PhotoUrl,
                WhatsappNumber = c.WhatsappNumber,
                PhoneNumber = c.PhoneNumber,
                Email = c.Email,
                CowRate = c.CowRate,
                BuffaloRate = c.BuffaloRate
            }).ToList();

            return new GeneralResponse<List<CustomerListDto>>
            {
                Success = true,
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                Message = "Customer list retrieved successfully",
                Data = result
            };
        }


    }
}
