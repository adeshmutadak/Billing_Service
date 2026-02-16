using CommonLayer.CommonResponse;
using Dto.Request;
using MilkBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ICustomerService
    {
        Task<GeneralResponse<List<CustomerListDto>>> GetAllCustomersAsync(int userId );

        Task<GeneralResponse<CustomerListDto>> AddCustomerAsync(AddCustomerRequestDto dto);

        Task<GeneralResponse<CustomerListDto>> UpdateCustomerAsync(UpdateCustomerRequestDto dto);
        Task<GeneralResponse<CustomerListDto>> GetCustomerByIdAsync(int id);
        Task<GeneralResponse<string>> DeleteCustomerAsync(int id);

        Task<GeneralResponse<List<Customer>>> SearchCustomersAsync(string name, string phoneNumber, string address);

    }
}
