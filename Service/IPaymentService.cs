using CommonLayer.CommonResponse;
using Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IPaymentService
    {
        public Task<BaseResponse> AddBill(AddBillDto addBillDto);
        Task<GeneralResponse<List<PaymentListDto>>> GetPaymentsAsync(
    int userId,
    int customerId,
    int? month,
    bool? isPaymentDone
);

    }
}
