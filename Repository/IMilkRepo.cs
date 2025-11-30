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

    }
}
