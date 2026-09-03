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
        Task<List<Customer>> GetCustomersAsync(int userId, int? customerId);

        // Entries and payments are scoped by customer ownership rather than by
        // milkentries.UserId / payments.UserId, because those columns have
        // drifted in the existing data: several entries for a customer owned by
        // user 1 are stamped user 2, and filtering on them would silently drop
        // rows. Ownership is established by the customer list above.
        Task<List<Milkentry>> GetEntriesForYearAsync(List<int> customerIds, int year);
        Task<List<Payment>> GetPaymentsForYearAsync(List<int> customerIds, int year);

        Task<List<int>> GetAvailableYearsAsync(List<int> customerIds);
    }
}
