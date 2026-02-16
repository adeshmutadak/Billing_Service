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
    public class PaymentRepo :IPaymentRepo
    {
        private readonly AppDbContext _context;

        public PaymentRepo(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Payment>> GetPaymentsAsync(
    int userId,
    int customerId,
    int? month,
    bool? isPaymentDone
)
        {
            var query = _context.Payments
                .Where(p =>
                    p.UserId == userId &&
                    p.CustomerId == customerId
                )
                .AsQueryable();

            if (month.HasValue)
            {
                query = query.Where(p =>
                    p.Date.HasValue &&
                    p.Date.Value.ToDateTime(TimeOnly.MinValue).Month == month.Value
                );
            }


            if (isPaymentDone.HasValue)
            {
                query = query.Where(p => p.IsPaymentDone == isPaymentDone.Value);
            }

            return await query.ToListAsync();
        }

    }
}
