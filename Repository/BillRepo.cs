using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilkBilling.Data;
using MilkBilling.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class BillRepo : IBillRepo
    {
        private readonly AppDbContext _context;

        public BillRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetCustomerAsync(int userId, int customerId)
        {
            return await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == customerId
                                       && c.UserId == userId
                                       && (c.IsDeleted == false || c.IsDeleted == null));
        }

        public async Task<List<Milkentry>> GetEntriesForMonthAsync(int customerId, int year, int month)
        {
            // Half-open window, so month length and leap years need no special case.
            var start = new DateOnly(year, month, 1);
            var end = start.AddMonths(1);

            return await _context.Milkentries
                .Where(m => m.CustomerId == customerId
                         && (m.IsDeleted == false || m.IsDeleted == null)
                         && m.Date >= start
                         && m.Date < end)
                .AsNoTracking()
                .OrderBy(m => m.Date)
                .ToListAsync();
        }

        public async Task<Bill?> GetBillAsync(int customerId, int year, int month)
        {
            // Tracked on purpose: the service updates this instance when a bill
            // for the period already exists.
            return await _context.Bills
                .FirstOrDefaultAsync(b => b.CustomerId == customerId
                                       && b.Year == year
                                       && b.Month == month
                                       && (b.IsDeleted == false || b.IsDeleted == null));
        }

        public async Task<Bill> AddBillAsync(Bill bill)
        {
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
            return bill;
        }

        public async Task UpdateBillAsync(Bill bill)
        {
            _context.Bills.Update(bill);
            await _context.SaveChangesAsync();
        }



        public async Task<List<Customer>> GetCustomersAsync(int userId, int? customerId, int? createdInYear)
        {
            var query = _context.Customers
                .Where(c => c.UserId == userId
                         && (c.IsDeleted == false || c.IsDeleted == null));

            if (customerId.HasValue)
                query = query.Where(c => c.CustomerId == customerId.Value);

            // Only applied when the caller opts in. The home screen passes the
            // current year, and scoping on creation year would empty its list the
            // moment the calendar year rolls over.
            if (createdInYear.HasValue)
                query = query.Where(c => c.CreatedAt != null
                                      && c.CreatedAt.Value.Year == createdInYear.Value);

            return await query.AsNoTracking()
                              .OrderBy(c => c.Name)
                              .ToListAsync();
        }

        public async Task<List<Milkentry>> GetEntriesForYearAsync(List<int> customerIds, int year)
        {
            if (customerIds == null || customerIds.Count == 0)
                return new List<Milkentry>();

            // Half-open window, so month lengths and leap years need no special case.
            var start = new DateOnly(year, 1, 1);
            var end = new DateOnly(year + 1, 1, 1);

            return await _context.Milkentries
                .Where(m => customerIds.Contains(m.CustomerId)
                         && (m.IsDeleted == false || m.IsDeleted == null)
                         && m.Date >= start
                         && m.Date < end)
                .AsNoTracking()
                .OrderBy(m => m.Date)
                .ThenBy(m => m.EntryId)
                .ToListAsync();
        }

        public async Task<List<Bill>> GetBillsForYearAsync(List<int> customerIds, int year)
        {
            if (customerIds == null || customerIds.Count == 0)
                return new List<Bill>();

            // bills carries Year and Month as integer columns, so no date maths.
            return await _context.Bills
                .Where(b => customerIds.Contains(b.CustomerId)
                         && b.Year == year
                         && (b.IsDeleted == false || b.IsDeleted == null))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Bill?> GetLatestEarlierBillAsync(int customerId, int year, int month)
        {
            // Everything strictly before (year, month), newest first.
            // Comparing on Year and Month as integers avoids any date arithmetic.
            return await _context.Bills
                .Where(b => b.CustomerId == customerId
                         && (b.IsDeleted == false || b.IsDeleted == null)
                         && (b.Year < year || (b.Year == year && b.Month < month)))
                .OrderByDescending(b => b.Year)
                .ThenByDescending(b => b.Month)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<List<Bill>> GetBillsAfterAsync(int customerId, int year, int month)
        {
            // Tracked on purpose: the service rewrites each of these in place when
            // an earlier month's payment changes.
            return await _context.Bills
                .Where(b => b.CustomerId == customerId
                         && (b.IsDeleted == false || b.IsDeleted == null)
                         && (b.Year > year || (b.Year == year && b.Month > month)))
                .OrderBy(b => b.Year)
                .ThenBy(b => b.Month)
                .ToListAsync();
        }
    }
}
