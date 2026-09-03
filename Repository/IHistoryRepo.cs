using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilkBilling.Models;

namespace Repository
{
    public interface IHistoryRepo
    {
        /// <summary>Customers owned by this user. When createdInYear is supplied,
        /// only those whose customers.CreatedAt falls in that year are returned.</summary>
        Task<List<Customer>> GetCustomersAsync(int userId, int? customerId, int? createdInYear);

        // Entries, bills and payments are scoped by customer ownership rather than
        // by their own UserId columns, because those have drifted in the existing
        // data: several entries for a customer owned by user 1 are stamped user 2.
        // Ownership is established by the customer list above.

        /// <summary>Milk entries in the year, keyed on milkentries.Date.</summary>
        Task<List<Milkentry>> GetEntriesForYearAsync(List<int> customerIds, int year);

        /// <summary>Generated bills, matched on the bills.Year and bills.Month columns.</summary>
        Task<List<Bill>> GetBillsForYearAsync(List<int> customerIds, int year);

        /// <summary>Payments in the year, keyed on payments.Date, which records
        /// the month a payment settles rather than when it was taken.</summary>
        Task<List<Payment>> GetPaymentsForYearAsync(List<int> customerIds, int year);

        /// <summary>Years that hold a milk entry, a bill or a payment, newest first.</summary>
        Task<List<int>> GetAvailableYearsAsync(List<int> customerIds);
    }
}
