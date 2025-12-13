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
    public class MilkService :IMilkService
    {
        private readonly IMilkRepo _milkRepo;
        private readonly ICustomerRepo _customerRepo;

        public MilkService(IMilkRepo milkRepo, ICustomerRepo customerRepo)
        {
            _milkRepo = milkRepo;
            _customerRepo = customerRepo;
        }

        public async Task<GeneralResponse<AddMilkEntryDto>> AddMilkEntryAsync(AddMilkEntryDto dto)
        {
            // 1️⃣ Fetch Customer
            var customer = await _customerRepo.GetCustomerByIdAsync(dto.CustomerId);
            if (customer == null)
            {
                return new GeneralResponse<AddMilkEntryDto>
                {
                    Success = false,
                    Message = "Customer not found",
                    HttpStatusCode = HttpStatusCode.NotFound
                };
            }

            var (cowRate, buffaloRate) = await _customerRepo.GetCustomerRatesAsync(dto.CustomerId);

            int finalCowRate = (dto.CowRate.HasValue && dto.CowRate.Value > 0)
                     ? Convert.ToInt32(dto.CowRate.Value)
                     : Convert.ToInt32(cowRate ?? 0);

            int finalBuffaloRate = (dto.BuffaloRate.HasValue && dto.BuffaloRate.Value > 0)
                                    ? Convert.ToInt32(dto.BuffaloRate.Value)
                                    : Convert.ToInt32(buffaloRate ?? 0);

            // Convert litres also to int
            int cowLitre = Convert.ToInt32(dto.CowLitre ?? 0);
            int buffaloLitre = Convert.ToInt32(dto.BuffaloLitre ?? 0);

            // Calculate total (integer values only)
            int todayTotal = (cowLitre * finalCowRate) + (buffaloLitre * finalBuffaloRate);


            // 4️⃣ If you want previous days total included:
            //    (You said 01/01 = 60 , 02/01 = previous+today)
            //var previousTotal = await _milkRepo.GetTotalAmountTillDate(dto.CustomerId, dto.Date);

            //decimal finalTotal = previousTotal + todayTotal;

            // 5️⃣ Create new Milk Entry
            var entry = new Milkentry
            {
                CustomerId = dto.CustomerId,
                UserId = dto.UserId,
                Date = dto.Date,
                CowLitre = dto.CowLitre,
                BuffaloLitre = dto.BuffaloLitre,
                CowRate = finalCowRate,
                BuffaloRate = finalBuffaloRate,
                TotalAmount = todayTotal,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var savedEntry = await _milkRepo.AddMilkEntryAsync(entry);

            // 6️⃣ Prepare Response
            var responseDto = new AddMilkEntryDto
            {
                EntryId = savedEntry.EntryId,
                CustomerId = savedEntry.CustomerId,
                Date = savedEntry.Date,
                CowLitre = savedEntry.CowLitre,
                BuffaloLitre = savedEntry.BuffaloLitre,
                CowRate = savedEntry.CowRate,
                BuffaloRate = savedEntry.BuffaloRate,
                TotalAmount = savedEntry.TotalAmount
            };

            return new GeneralResponse<AddMilkEntryDto>
            {
                Success = true,
                Message = "Milk entry added successfully",
                HttpStatusCode = HttpStatusCode.Created,
                Data = responseDto
            };
        }




        public async Task<GeneralResponse<List<MilkEntryResponseDto>>> GetMilkEntriesByCustomerAndUserAsync(int customerId, int userId)
        {
            var customer = await _customerRepo.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                return new GeneralResponse<List<MilkEntryResponseDto>>
                {
                    Success = false,
                    Message = "Customer not found",
                    HttpStatusCode = HttpStatusCode.NotFound
                };
            }

            var entries = await _milkRepo.GetMilkEntriesByCustomerAndUserAsync(customerId, userId);

            if (entries == null || entries.Count == 0)
            {
                return new GeneralResponse<List<MilkEntryResponseDto>>
                {
                    Success = false,
                    Message = "No milk entries found",
                    HttpStatusCode = HttpStatusCode.NotFound
                };
            }

            // Convert to DTO
            var mappedEntries = entries.Select(x => new MilkEntryResponseDto
            {
                EntryId = x.EntryId,
                CustomerId = x.CustomerId,
                UserId = x.UserId,
                Date = x.Date,
                CowLitre = x.CowLitre,
                BuffaloLitre = x.BuffaloLitre,
                CowRate = x.CowRate,
                BuffaloRate = x.BuffaloRate,
                TotalAmount = x.TotalAmount
            }).ToList();

            return new GeneralResponse<List<MilkEntryResponseDto>>
            {
                Success = true,
                Message = "Milk entries retrieved successfully",
                HttpStatusCode = HttpStatusCode.OK,
                Data = mappedEntries
            };
        }

        //Task<GeneralResponse<List<Milkentry>>> IMilkService.GetMilkEntriesByCustomerAndUserAsync(int customerId, int userId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
