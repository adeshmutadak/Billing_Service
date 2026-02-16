using CommonLayer.CommonResponse;
using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace MilkBilling.Controllers
{

    [Route("v1/api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService; 
        }


        [HttpPost]
        [Authorize]
        public async  Task<BaseResponse> AddBill(AddBillDto addBillDto)
        {
            var response= await _paymentService.AddBill(addBillDto);
            return response;
        }

        [Authorize]
        [HttpGet("payments")]
        public async Task<GeneralResponse<List<PaymentListDto>>> GetPayments(
     int customerId,
     int? month,
     bool? isPaymentDone
 )
        {
            int userId = int.Parse(User.FindFirst("UserId")!.Value);

            return await _paymentService.GetPaymentsAsync(
                userId,
                customerId,
                month,
                isPaymentDone
            );
        }

    }
}
