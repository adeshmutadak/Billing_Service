using CommonLayer.CommonResponse;
using Dto.Request;
using MilkBilling.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepo _paymentRepo;
        private readonly ICustomerRepo _customerRepo;
        public PaymentService(IPaymentRepo paymentRepo ,ICustomerRepo customerRepo)
        {
            _paymentRepo = paymentRepo;
            _customerRepo = customerRepo;
        }

        public async Task<BaseResponse> AddBill(AddBillDto addBillDto)
        {
            var customer = await _customerRepo.GetCustomerByIdAsync(addBillDto.CustomerId);
            if(customer == null)
            {
                 return new BaseResponse
                {
                    Message = "Customer Not found .",
                    Success = false,
                    HttpStatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            var payment = new Payment
            {
                UserId = addBillDto.CustomerId,
                CustomerId = addBillDto.UserId,
                PaymentType = addBillDto.PaymentType,
                Amount = addBillDto.Amount,
                Remaning = addBillDto.Remaning,
                Date = addBillDto.Date,
                IsPaymentDone = addBillDto.IsPaymentDone,
                CreatedAt=DateTime.Now ,
                UpdatedAt=DateTime.Now ,
            };

            await _paymentRepo.AddAsync(payment);
            return new BaseResponse
            {
                Message = "Bill Added Successfully",
                Success = true,
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };
        }

        public async Task<GeneralResponse<List<PaymentListDto>>> GetPaymentsAsync(
    int userId,
    int customerId,
    int? month,
    bool? isPaymentDone
)
        {
            var payments = await _paymentRepo.GetPaymentsAsync(
                userId,
                customerId,
                month,
                isPaymentDone
            );

            var result = payments.Select(p => new PaymentListDto
            {
                PaymentId = p.PaymentId,
                CustomerId = p.CustomerId,
                Amount = p.Amount,
                Remaning = p.Remaning,
                PaymentType = p.PaymentType,
                Date = p.Date,
                IsPaymentDone = p.IsPaymentDone
            }).ToList();

            return new GeneralResponse<List<PaymentListDto>>
            {
                Success = true,
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Payments retrieved successfully",
                Data = result
            };
        }

    }
}
