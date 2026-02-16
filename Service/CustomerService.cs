using CommonLayer.CommonResponse;
using CommonLayer.PhotoUpload;
using Dto.Request;
using MilkBilling.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public async Task<GeneralResponse<List<CustomerListDto>>> GetAllCustomersAsync(int userId  )
        {
            var customers = await _customerRepo.GetAllCustomersAsync(userId);

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
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Customer list retrieved successfully",
                Data = result
            };
        }


        public async Task<GeneralResponse<CustomerListDto>> AddCustomerAsync(AddCustomerRequestDto dto)
        {

            string? photoUrl = null;

            // Save Image If Provided
            if (!string.IsNullOrEmpty(dto.PhotoUrl))
            {

                string folderPath = "D:\\Projects\\.Vs_Source\\Push_milk\\Billing_Service\\DTO\\Uploads\\Customer\\";  // your upload folder
                photoUrl = await _fileService.SaveBase64ImageAsync(dto.PhotoUrl, folderPath);
            }




            var newCustomer = new Customer
            {
                UserId = dto.UserId,
                Name = dto.Name,
                Address = dto.Address,
                PhotoUrl = photoUrl,
                WhatsappNumber = dto.WhatsappNumber,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                CowRate = dto.CowRate,
                BuffaloRate = dto.BuffaloRate,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var savedCustomer = await _customerRepo.AddCustomerAsync(newCustomer);

            var responseDto = new CustomerListDto
            {
                CustomerId = savedCustomer.CustomerId,
                Name = savedCustomer.Name,
                //Address = savedCustomer.Address,
                //PhotoUrl = savedCustomer.PhotoUrl,
                //WhatsappNumber = savedCustomer.WhatsappNumber,
                //PhoneNumber = savedCustomer.PhoneNumber,
                //Email = savedCustomer.Email,
                //CowRate = savedCustomer.CowRate,
                //BuffaloRate = savedCustomer.BuffaloRate
            };

            return new GeneralResponse<CustomerListDto>
            {
                Success = true,
                HttpStatusCode = System.Net.HttpStatusCode.Created,
                Message = "Customer added successfully",
                Data = responseDto
            };
        }

        public async Task<GeneralResponse<CustomerListDto>> UpdateCustomerAsync(UpdateCustomerRequestDto dto)
        {
            var customer = await _customerRepo.GetCustomerByIdAsync(dto.CustomerId);

            if (customer == null)
            {
                return new GeneralResponse<CustomerListDto>
                {
                    Success = false,
                    HttpStatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = "Customer not found"
                };
            }

            // If Base64 photo is provided, save new photo
            if (!string.IsNullOrEmpty(dto.Base64Photo))
            {
                string folderPath = "D:\\Projects\\.Vs_Source\\Push_milk\\Billing_Service\\DTO\\Uploads\\Customer\\";
                customer.PhotoUrl = await _fileService.SaveBase64ImageAsync(dto.Base64Photo, folderPath);
            }

            // Update only provided fields
            if (dto.Name != null)
                customer.Name = dto.Name;

            if (dto.Address != null)
                customer.Address = dto.Address;

            if (dto.WhatsappNumber != null)
                customer.WhatsappNumber = dto.WhatsappNumber;

            if (dto.PhoneNumber != null)
                customer.PhoneNumber = dto.PhoneNumber;

            if (dto.Email != null)
                customer.Email = dto.Email;

            if (dto.CowRate.HasValue)
                customer.CowRate = dto.CowRate.Value;

            if (dto.BuffaloRate.HasValue)
                customer.BuffaloRate = dto.BuffaloRate.Value;

            customer.UpdatedAt = DateTime.Now;

            await _customerRepo.UpdateCustomerAsync(customer);

            // Return DTO
            var responseDto = new CustomerListDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Address = customer.Address,
                PhotoUrl = customer.PhotoUrl,
                WhatsappNumber = customer.WhatsappNumber,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                CowRate = customer.CowRate,
                BuffaloRate = customer.BuffaloRate
            };

            return new GeneralResponse<CustomerListDto>
            {
                Success = true,
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                Message = "Customer updated successfully",
                Data = responseDto
            };
        }


        public async Task<GeneralResponse<CustomerListDto>> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepo.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return new GeneralResponse<CustomerListDto>
                {
                    Success = false,
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Customer not found"
                };
            }

            var result = new CustomerListDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Address = customer.Address,
                PhotoUrl = customer.PhotoUrl,
                WhatsappNumber = customer.WhatsappNumber,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                CowRate = customer.CowRate,
                BuffaloRate = customer.BuffaloRate
            };

            return new GeneralResponse<CustomerListDto>
            {
                Success = true,
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Customer retrieved successfully",
                Data = result
            };
        }



        public async Task<GeneralResponse<string>> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepo.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Customer not found"
                };
            }

            customer.IsDeleted = true;
            customer.UpdatedAt = DateTime.Now;

            await _customerRepo.UpdateCustomerAsync(customer);

            return new GeneralResponse<string>
            {
                Success = true,
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Customer deleted successfully",
                Data = "Deleted"
            };
        }


        public async Task<GeneralResponse<List<Customer>>> SearchCustomersAsync(string name, string phoneNumber, string address)
        {
            var customers = await _customerRepo.SearchCustomersAsync(name, phoneNumber, address);

            if (customers == null || customers.Count == 0)
            {
                return new GeneralResponse<List<Customer>>
                {
                    Success = false,
                    Message = "No customers found",
                    HttpStatusCode = HttpStatusCode.NotFound
                };
            }

            return new GeneralResponse<List<Customer>>
            {
                Success = true,
                Message = "Customers fetched successfully",
                HttpStatusCode = HttpStatusCode.OK,
                Data = customers
            };
        }

    }
}
