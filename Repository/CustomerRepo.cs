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
    }
}
