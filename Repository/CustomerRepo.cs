using Microsoft.EntityFrameworkCore;
using MilkBilling.Data;
using MilkBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CustomerRepo :ICustomerRepo
    {

        private readonly AppDbContext _context;

        public CustomerRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Where(c => c.IsDeleted == false || c.IsDeleted == null)
                .ToListAsync();
        }
        public async Task<List<Customer>> GetAllCustomersAsync(int userId)
        {
            var startOfYear = new DateTime(DateTime.Now.Year, 1, 1);
            var startOfNextYear = startOfYear.AddYears(1);

            return await _context.Customers
                .Where(c =>
                    (c.IsDeleted == false || c.IsDeleted == null) &&
                    c.UserId == userId &&
                    c.CreatedAt >= startOfYear &&
                    c.CreatedAt < startOfNextYear
                )
                .ToListAsync();
        }




        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }


        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == id && (c.IsDeleted == false || c.IsDeleted == null));
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }



        //Used in the Milk entreis
        public async Task<(decimal? CowRate, decimal? BuffaloRate)> GetCustomerRatesAsync(int customerId)
        {
            var customer = await _context.Customers
                .Where(c => c.CustomerId == customerId && (c.IsDeleted == false || c.IsDeleted == null))
                .Select(c => new { c.CowRate, c.BuffaloRate })
                .FirstOrDefaultAsync();

            if (customer == null)
                return (null, null);

            return (customer.CowRate, customer.BuffaloRate);
        }



        public async Task<List<Customer>> SearchCustomersAsync(string name, string phoneNumber, string address)
        {
            var query = _context.Customers
                .Where(c => c.IsDeleted == false || c.IsDeleted == null)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(c => c.Name.ToLower().Contains(name.ToLower()));

            if (!string.IsNullOrEmpty(phoneNumber))
                query = query.Where(c => c.PhoneNumber.Contains(phoneNumber));

            if (!string.IsNullOrEmpty(address))
                query = query.Where(c => c.Address.ToLower().Contains(address.ToLower()));

            return await query.ToListAsync();
        }


    }
}
