using MilkBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ICustomerRepo
    {
        Task<List<Customer>> GetAllCustomersAsync(int userId);

        Task<List<Customer>> GetAllCustomersAsync();
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task UpdateCustomerAsync(Customer customer);
        Task<(decimal? CowRate, decimal? BuffaloRate)> GetCustomerRatesAsync(int customerId);

        Task<List<Customer>> SearchCustomersAsync(string name, string phoneNumber, string address);

        
    }
}
