using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilkBilling.Models;

namespace Repository
{
    public interface IBillRepo
    {
        /// <summary>The customer, only if owned by this user. Returns null otherwise,
        /// so ownership and existence are one check.</summary>
        Task<Customer?> GetCustomerAsync(int userId, int customerId);

        /// <summary>Milk entries for the month, keyed on milkentries.Date.
        /// Scoped by CustomerId rather than milkentries.UserId, which has drifted
        /// in the existing data; ownership is established by GetCustomerAsync.</summary>
        Task<List<Milkentry>> GetEntriesForMonthAsync(int customerId, int year, int month);

        /// <summary>The existing bill for this period, if one was already generated.</summary>
        Task<Bill?> GetBillAsync(int customerId, int year, int month);

        Task<Bill> AddBillAsync(Bill bill);
        Task UpdateBillAsync(Bill bill);

        Task<Bill?> GetLatestEarlierBillAsync(int customerId, int year, int month);

        Task<List<Customer>> GetCustomersAsync(int userId, int? customerId, int? createdInYear);

        /// <summary>Every milk entry for these customers in the year, ascending by
        /// date then entry id.</summary>
        Task<List<Milkentry>> GetEntriesForYearAsync(List<int> customerIds, int year);

        /// <summary>Every bill for these customers in the year, matched on the
        /// bills.Year and bills.Month columns.</summary>
        Task<List<Bill>> GetBillsForYearAsync(List<int> customerIds, int year);

        Task<List<Bill>> GetBillsAfterAsync(int customerId, int year, int month);
    }
}
