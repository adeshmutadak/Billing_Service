using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MilkBilling.Data;
using MilkBilling.Models;

namespace Repository
{
    public class HistoryRepo : IHistoryRepo
    {
        private readonly AppDbContext _context;

        public HistoryRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetCustomersAsync(int userId, int? customerId, int? createdInYear)
        {
            var query = _context.Customers
                .Where(c => c.UserId == userId
                         && (c.IsDeleted == false || c.IsDeleted == null));

            if (customerId.HasValue)
                query = query.Where(c => c.CustomerId == customerId.Value);

            // Scoping on the year the customer was created, as specified. A
            // customer created earlier is therefore absent from a later year even
            // if they have deliveries in it; the service exposes a flag to lift
            // this restriction.
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

            var start = new DateOnly(year, 1, 1);
            var end = new DateOnly(year + 1, 1, 1);

            return await _context.Milkentries
                .Where(m => customerIds.Contains(m.CustomerId)
                         && (m.IsDeleted == false || m.IsDeleted == null)
                         && m.Date >= start
                         && m.Date < end)
                .AsNoTracking()
                .OrderBy(m => m.Date)
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

        public async Task<List<Payment>> GetPaymentsForYearAsync(List<int> customerIds, int year)
        {
            if (customerIds == null || customerIds.Count == 0)
                return new List<Payment>();

            var start = new DateOnly(year, 1, 1);
            var end = new DateOnly(year + 1, 1, 1);

            // Filtered on Date, not CreatedAt: Date is the month the payment
            // settles, CreatedAt is only when it was entered.
            return await _context.Payments
                .Where(p => customerIds.Contains(p.CustomerId)
                         && (p.IsDeleted == false || p.IsDeleted == null)
                         && p.Date != null
                         && p.Date >= start
                         && p.Date < end)
                .AsNoTracking()
                .OrderBy(p => p.Date)
                .ToListAsync();
        }

        public async Task<List<int>> GetAvailableYearsAsync(List<int> customerIds)
        {
            if (customerIds == null || customerIds.Count == 0)
                return new List<int>();

            // Dates are projected first and the year taken in memory, so this does
            // not depend on DateOnly-to-YEAR() SQL translation. No AsNoTracking
            // here: it is constrained to reference types, and a projection to a
            // scalar such as DateOnly is never tracked in the first place.
            var entryDates = await _context.Milkentries
                .Where(m => customerIds.Contains(m.CustomerId)
                         && (m.IsDeleted == false || m.IsDeleted == null))
                .Select(m => m.Date)
                .ToListAsync();

            var paymentDates = await _context.Payments
                .Where(p => customerIds.Contains(p.CustomerId)
                         && (p.IsDeleted == false || p.IsDeleted == null)
                         && p.Date != null)
                .Select(p => p.Date)
                .ToListAsync();

            var billYears = await _context.Bills
                .Where(b => customerIds.Contains(b.CustomerId)
                         && (b.IsDeleted == false || b.IsDeleted == null))
                .Select(b => b.Year)
                .ToListAsync();

            return entryDates.Select(d => d.Year)
                .Concat(paymentDates.Where(d => d.HasValue).Select(d => d.Value.Year))
                .Concat(billYears)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();
        }
    }
}
