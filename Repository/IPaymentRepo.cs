using MilkBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IPaymentRepo
    {
        Task AddAsync(Payment payment);
        //Task<Payment> GetBasedOnCustomerId
        Task<List<Payment>> GetPaymentsAsync(
    int userId,
    int customerId,
    int? month,
    bool? isPaymentDone
);

    }
}
