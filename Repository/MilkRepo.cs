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

        public async Task<List<Milkentry>> GetMilkEntriesByCustomerAndUserAsync(int customerId, int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var startOfMonth = new DateOnly(today.Year, today.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            return await _context.Milkentries
                .Where(x => x.CustomerId == customerId
                         && x.UserId == userId
                         && (x.IsDeleted == false || x.IsDeleted == null)
                         && x.Date >= startOfMonth
                         && x.Date < startOfNextMonth)
                .AsNoTracking()
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }


        public async Task<Milkentry?> GetMilkEntryByIdAsync(
    int customerId,
    int userId,
    int entryId)
        {
            return await _context.Milkentries
                .FirstOrDefaultAsync(x =>
                    x.EntryId == entryId &&
                    x.CustomerId == customerId &&
                    x.UserId == userId &&
                    (x.IsDeleted == false || x.IsDeleted == null));
        }
        public async Task UpdateMilkEntryAsync(Milkentry entry)
        {
            _context.Milkentries.Update(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<Milkentry?> GetMilkEntryByEntryIdAsync(int entryId)
        {
            return await _context.Milkentries
                .FirstOrDefaultAsync(x =>
                    x.EntryId == entryId &&
                    (x.IsDeleted == false || x.IsDeleted == null));
        }
        public async Task DeleteMilkEntryAsync(Milkentry entry)
        {
            _context.Milkentries.Update(entry);
            await _context.SaveChangesAsync();
        }

    }
}
