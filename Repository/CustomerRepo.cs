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
            return await _context.Customers
                .Where(c =>
                    (c.IsDeleted == false || c.IsDeleted == null) &&
                     c.UserId == userId
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

    }
}
