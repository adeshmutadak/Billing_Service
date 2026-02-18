using CommonLayer.CommonResponse;
using Dto.Request;
using MilkBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IMilkRepo
    {
        Task<decimal> GetTotalAmountTillDate(int customerId, DateOnly date);
        Task<Milkentry> AddMilkEntryAsync(Milkentry entry);

        Task<List<Milkentry>> GetMilkEntriesByCustomerAndUserAsync(int customerId, int userId);

        Task<Milkentry?> GetMilkEntryByIdAsync(int customerId, int userId, int entryId);
        Task UpdateMilkEntryAsync(Milkentry entry);

        Task<Milkentry?> GetMilkEntryByEntryIdAsync(int entryId);
        Task DeleteMilkEntryAsync(Milkentry entry);


    }
}
