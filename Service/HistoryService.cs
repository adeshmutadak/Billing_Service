using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.CommonResponse;
using Dto.Request;
using Dto.Response;
using MilkBilling.Models;
using Repository;

namespace Service
{
    public class HistoryService : IHistoryService
    {
        private const string StatusPaid = "Paid";
        private const string StatusPartial = "Partial";
        private const string StatusUnpaid = "Unpaid";
        private const string StatusNoActivity = "NoActivity";

        private readonly IHistoryRepo _historyRepo;

        public HistoryService(IHistoryRepo historyRepo)
        {
            _historyRepo = historyRepo;
        }

        public async Task<GeneralResponse<HistoryResponseDto>> GetHistoryAsync(
            int userId, HistoryFilterDto filter)
        {
            if (filter.Year < 2000 || filter.Year > DateTime.Now.Year + 1)
            {
                return Fail("Invalid year", HttpStatusCode.BadRequest);
            }

            if (filter.Month.HasValue && (filter.Month < 1 || filter.Month > 12))
            {
                return Fail("Month must be between 1 and 12", HttpStatusCode.BadRequest);
            }

            var customers = await _historyRepo.GetCustomersAsync(userId, filter.CustomerId);
            if (customers.Count == 0)
            {
                return Fail("No customers found", HttpStatusCode.NotFound);
            }

            var customerIds = customers.Select(c => c.CustomerId).ToList();

            // The whole year is always read, so the totals stay correct even when
            // only one month is being displayed.
            var entries = await _historyRepo.GetEntriesForYearAsync(customerIds, filter.Year);
            var payments = await _historyRepo.GetPaymentsForYearAsync(customerIds, filter.Year);

            var entriesByCustomer = entries.ToLookup(e => e.CustomerId);
            var paymentsByCustomer = payments.ToLookup(p => p.CustomerId);

            var includeDays = filter.IncludeDetail || filter.Month.HasValue;

            var customerDtos = new List<HistoryCustomerDto>(customers.Count);
            var yearMonths = new List<HistoryMonthDto>(customers.Count * 12);

            var activeCustomers = 0;
            var paidCustomers = 0;
            var unpaidCustomers = 0;

            foreach (var customer in customers)
            {
                var custEntries = entriesByCustomer[customer.CustomerId].ToList();
                var custPayments = paymentsByCustomer[customer.CustomerId].ToList();

                // All twelve months are built every time, so the UI has a fixed
                // grid and an empty month is explicit rather than missing.
                var months = Enumerable.Range(1, 12)
                    .Select(m => BuildMonth(
                        filter.Year, m, custEntries, custPayments,
                        includeDays, filter.IncludeEmptyDays))
                    .ToList();

                yearMonths.AddRange(months);

                var activeMonths = months.Where(m => m.PaymentStatus != StatusNoActivity).ToList();
                if (activeMonths.Count > 0)
                {
                    activeCustomers++;
                    if (activeMonths.All(m => m.IsPaid)) paidCustomers++;
                    else unpaidCustomers++;
                }

                // Filters trim the month list. The customer is still returned, so
                // the response shape is predictable; hide an empty Months array
                // in the UI if you do not want the row.
                IEnumerable<HistoryMonthDto> visible = months;

                if (filter.Month.HasValue)
                    visible = visible.Where(m => m.Month == filter.Month.Value);

                if (filter.IsPaid.HasValue)
                    visible = visible.Where(m => m.IsPaid == filter.IsPaid.Value);

                customerDtos.Add(new HistoryCustomerDto
                {
                    CustomerId = customer.CustomerId,
                    Name = customer.Name,
                    PhoneNumber = customer.PhoneNumber,
                    WhatsappNumber = customer.WhatsappNumber,
                    Address = customer.Address,
                    CowRate = customer.CowRate,
                    BuffaloRate = customer.BuffaloRate,
                    Months = visible.ToList()
                });
            }

            var totalAmount = yearMonths.Sum(m => m.TotalAmount);
            var paidAmount = yearMonths.Sum(m => m.PaidAmount);
            var remaining = totalAmount - paidAmount;

            return new GeneralResponse<HistoryResponseDto>
            {
                Success = true,
                Message = activeCustomers == 0
                    ? "No history found for the selected filters"
                    : "History retrieved successfully",
                HttpStatusCode = HttpStatusCode.OK,
                Data = new HistoryResponseDto
                {
                    Totals = new HistoryTotalsDto
                    {
                        CustomerCount = activeCustomers,
                        PaidCustomerCount = paidCustomers,
                        UnpaidCustomerCount = unpaidCustomers,
                        TotalAmount = totalAmount,
                        PaidAmount = paidAmount,
                        RemainingAmount = remaining > 0 ? remaining : 0
                    },
                    Customers = customerDtos
                }
            };
        }

        public async Task<GeneralResponse<HistoryFilterOptionsDto>> GetFilterOptionsAsync(int userId)
        {
            var customers = await _historyRepo.GetCustomersAsync(userId, null);
            var years = await _historyRepo.GetAvailableYearsAsync(
                customers.Select(c => c.CustomerId).ToList());

            return new GeneralResponse<HistoryFilterOptionsDto>
            {
                Success = true,
                Message = "Filter options retrieved successfully",
                HttpStatusCode = HttpStatusCode.OK,
                Data = new HistoryFilterOptionsDto
                {
                    Years = years,
                    Customers = customers.Select(c => new HistoryCustomerOptionDto
                    {
                        CustomerId = c.CustomerId,
                        Name = c.Name,
                        PhoneNumber = c.PhoneNumber
                    }).ToList()
                }
            };
        }

        private static HistoryMonthDto BuildMonth(
            int year,
            int month,
            List<Milkentry> entries,
            List<Payment> payments,
            bool includeDays,
            bool includeEmptyDays)
        {
            var monthEntries = entries
                .Where(e => e.Date.Year == year && e.Date.Month == month)
                .OrderBy(e => e.Date)
                .ToList();

            var monthPayments = payments
                .Where(p => p.Date.HasValue && p.Date.Value.Year == year && p.Date.Value.Month == month)
                .OrderBy(p => p.Date)
                .ToList();

            // TotalAmount is reported as stored, not recalculated from litres and
            // rates, so the history can never disagree with the printed bill.
            var totalAmount = monthEntries.Sum(e => e.TotalAmount ?? 0);
            var paidAmount = monthPayments.Sum(p => p.Amount);
            var remaining = totalAmount - paidAmount;

            string status;
            if (totalAmount <= 0 && paidAmount <= 0) status = StatusNoActivity;
            else if (paidAmount > 0 && remaining <= 0) status = StatusPaid;
            else if (paidAmount > 0) status = StatusPartial;
            else status = StatusUnpaid;

            var dto = new HistoryMonthDto
            {
                Month = month,
                MonthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month),
                TotalCowLitre = monthEntries.Sum(e => e.CowLitre ?? 0),
                TotalBuffaloLitre = monthEntries.Sum(e => e.BuffaloLitre ?? 0),
                TotalAmount = totalAmount,
                PaidAmount = paidAmount,
                RemainingAmount = remaining > 0 ? remaining : 0,
                PaymentStatus = status,
                IsPaid = status == StatusPaid
            };

            if (!includeDays)
            {
                return dto;
            }

            dto.Days = includeEmptyDays
                ? BuildFullMonth(year, month, monthEntries)
                : monthEntries.Select(ToDay).ToList();

            dto.Payments = monthPayments.Select(p => new HistoryPaymentDto
            {
                PaymentId = p.PaymentId,
                Date = p.Date,
                Amount = p.Amount,
                Remaining = p.Remaning,
                PaymentType = p.PaymentType,
                IsPaymentDone = p.IsPaymentDone ?? false
            }).ToList();

            return dto;
        }

        /// <summary>Every day of the month, zero-filled where there was no
        /// delivery. February 2024 yields 29 rows; February 2025 yields 28.
        /// A padded day carries EntryId null.</summary>
        private static List<HistoryDayDto> BuildFullMonth(
            int year, int month, List<Milkentry> monthEntries)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var byDay = monthEntries.ToLookup(e => e.Date.Day);
            var days = new List<HistoryDayDto>(daysInMonth);

            for (var day = 1; day <= daysInMonth; day++)
            {
                var entry = byDay[day].FirstOrDefault();

                if (entry != null)
                {
                    days.Add(ToDay(entry));
                    continue;
                }

                var date = new DateOnly(year, month, day);
                days.Add(new HistoryDayDto
                {
                    EntryId = null,
                    Date = date,
                    DayOfMonth = day,
                    DayName = date.DayOfWeek.ToString()
                });
            }

            return days;
        }

        private static HistoryDayDto ToDay(Milkentry e) => new HistoryDayDto
        {
            EntryId = e.EntryId,
            Date = e.Date,
            DayOfMonth = e.Date.Day,
            DayName = e.Date.DayOfWeek.ToString(),
            CowLitre = e.CowLitre ?? 0,
            BuffaloLitre = e.BuffaloLitre ?? 0,
            CowRate = e.CowRate ?? 0,
            BuffaloRate = e.BuffaloRate ?? 0,
            TotalAmount = e.TotalAmount ?? 0
        };

        private static GeneralResponse<HistoryResponseDto> Fail(string message, HttpStatusCode code) =>
            new GeneralResponse<HistoryResponseDto>
            {
                Success = false,
                Message = message,
                HttpStatusCode = code
            };
    }
}
