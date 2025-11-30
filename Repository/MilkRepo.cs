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
    public class MilkRepo :IMilkRepo
    {
        private readonly AppDbContext _context;

        public MilkRepo(AppDbContext context)
        {
            _context = context;
        }



        public async Task<decimal> GetTotalAmountTillDate(int customerId, DateOnly date)
        {
            return await _context.Milkentries
                .Where(m => m.CustomerId == customerId && m.Date < date)
                .SumAsync(m => m.TotalAmount ?? 0);
        }


        public async Task<Milkentry> AddMilkEntryAsync(Milkentry entry)
        {
            _context.Milkentries.Add(entry);
            await _context.SaveChangesAsync();
            return entry;
        }
    }
}
