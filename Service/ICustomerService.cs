using CommonLayer.CommonResponse;
using Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ICustomerService
    {
        Task<GeneralResponse<List<CustomerListDto>>> GetAllCustomersAsync();
    }
}
