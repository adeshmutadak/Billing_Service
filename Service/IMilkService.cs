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
    public interface IMilkService
    {

        Task<GeneralResponse<AddMilkEntryDto>> AddMilkEntryAsync(AddMilkEntryDto dto);
        Task<GeneralResponse<List<MilkEntryResponseDto>>> GetMilkEntriesByCustomerAndUserAsync(int customerId, int userId);


    }
}
