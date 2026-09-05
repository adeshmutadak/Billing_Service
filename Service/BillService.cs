using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.CommonResponse;
using Dto.Request;
using MilkBilling.Models;
using Repository;

namespace Service
{
    public class BillService : IBillService
    {
        private readonly IBillRepo _billRepo;

        public BillService(IBillRepo billRepo)
        {
            _billRepo = billRepo;
        }

        public async Task<GeneralResponse<BillResponseDto>> AddBillAsync(int userId, AddBillRequestDto dto)
        {
            if (dto == null)
            {
                return Fail("Request body is required", HttpStatusCode.BadRequest);
            }

            if (dto.Month < 1 || dto.Month > 12)
            {
                return Fail("Month must be between 1 and 12", HttpStatusCode.BadRequest);
            }

            if (dto.Year < 2000 || dto.Year > DateTime.Now.Year + 1)
            {
                return Fail("Invalid year", HttpStatusCode.BadRequest);
            }

            if (dto.PreviousBalance.HasValue && dto.PreviousBalance.Value < 0)
            {
                return Fail("Previous balance cannot be negative", HttpStatusCode.BadRequest);
            }

            // Ownership and existence in one check: the customer is only returned
            // when it belongs to the user in the token.
            var customer = await _billRepo.GetCustomerAsync(userId, dto.CustomerId);
            if (customer == null)
            {
                return Fail("Customer not found", HttpStatusCode.NotFound);
            }

            // Litres and amount always come from the register, never from the
            // request, so a bill cannot disagree with the daily entries.
            var entries = await _billRepo.GetEntriesForMonthAsync(dto.CustomerId, dto.Year, dto.Month);

            var totalCowLitre = entries.Sum(e => e.CowLitre ?? 0);
            var totalBuffaloLitre = entries.Sum(e => e.BuffaloLitre ?? 0);

            // Stored TotalAmount is summed rather than recomputed from litres and
            // rates, so the bill matches what the entry screen already showed.
            var totalAmount = entries.Sum(e => e.TotalAmount ?? 0);

            var previousBalance = dto.PreviousBalance ?? 0m;
            var totalPayable = totalAmount + previousBalance;

            if (entries.Count == 0 && previousBalance <= 0)
            {
                return Fail(
                    $"No milk entries for {MonthName(dto.Month)} {dto.Year} and no previous balance, so there is nothing to bill",
                    HttpStatusCode.BadRequest);
            }

            var existing = await _billRepo.GetBillAsync(dto.CustomerId, dto.Year, dto.Month);

            if (existing != null)
            {
                // A settled bill is not overwritten. Regenerating it would change
                // the amount a customer has already paid against.
                if (existing.IsPaymentDone)
                {
                    return Fail(
                        $"Bill {existing.BillId} for {MonthName(dto.Month)} {dto.Year} is already marked paid and cannot be regenerated",
                        HttpStatusCode.Conflict);
                }

                existing.TotalCowLitre = totalCowLitre;
                existing.TotalBuffaloLitre = totalBuffaloLitre;
                existing.TotalAmount = totalAmount;
                existing.PreviousBalance = previousBalance;
                existing.TotalPayable = totalPayable;
                existing.PaymentType = dto.PaymentType;
                existing.IsPaymentDone = dto.IsPaymentDone;
                existing.UpdatedAt = DateTime.Now;

                await _billRepo.UpdateBillAsync(existing);

                return Ok(existing, customer.Name, entries.Count, regenerated: true,
                    $"Bill for {MonthName(dto.Month)} {dto.Year} recalculated successfully");
            }

            var bill = new Bill
            {
                CustomerId = customer.CustomerId,

                // From the token, not the request body.
                UserId = userId,

                Month = dto.Month,
                Year = dto.Year,
                TotalCowLitre = totalCowLitre,
                TotalBuffaloLitre = totalBuffaloLitre,
                TotalAmount = totalAmount,
                PreviousBalance = previousBalance,
                TotalPayable = totalPayable,
                PaymentType = dto.PaymentType,
                IsPaymentDone = dto.IsPaymentDone,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var saved = await _billRepo.AddBillAsync(bill);

            return Ok(saved, customer.Name, entries.Count, regenerated: false,
                $"Bill for {MonthName(dto.Month)} {dto.Year} generated successfully",
                HttpStatusCode.Created);
        }

        private static string MonthName(int month) =>
            CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month);

        private static GeneralResponse<BillResponseDto> Ok(
            Bill bill,
            string customerName,
            int entryCount,
            bool regenerated,
            string message,
            HttpStatusCode code = HttpStatusCode.OK) =>
            new GeneralResponse<BillResponseDto>
            {
                Success = true,
                Message = message,
                HttpStatusCode = code,
                Data = new BillResponseDto
                {
                    BillId = bill.BillId,
                    CustomerId = bill.CustomerId,
                    CustomerName = customerName,
                    Month = bill.Month,
                    MonthName = MonthName(bill.Month),
                    Year = bill.Year,
                    EntryCount = entryCount,
                    TotalCowLitre = bill.TotalCowLitre ?? 0,
                    TotalBuffaloLitre = bill.TotalBuffaloLitre ?? 0,
                    TotalAmount = bill.TotalAmount ?? 0,
                    PreviousBalance = bill.PreviousBalance ?? 0,
                    TotalPayable = bill.TotalPayable ?? 0,
                    PaymentType = bill.PaymentType,
                    IsPaymentDone = bill.IsPaymentDone,
                    CreatedAt = bill.CreatedAt,
                    UpdatedAt = bill.UpdatedAt,
                    Regenerated = regenerated
                }
            };

        private static GeneralResponse<BillResponseDto> Fail(string message, HttpStatusCode code) =>
            new GeneralResponse<BillResponseDto>
            {
                Success = false,
                Message = message,
                HttpStatusCode = code
            };



        private const string StatusPaid = "Paid";
        private const string StatusUnpaid = "Unpaid";
        private const string StatusNoActivity = "NoActivity";

        public async Task<GeneralResponse<BillHistoryResponseDto>> GetHistoryAsync(
            int userId, BillHistoryFilterDto filter)
        {
            if (filter == null)
            {
                return HistoryFail("Request is required", HttpStatusCode.BadRequest);
            }

           

            if (filter.Month.HasValue && (filter.Month < 1 || filter.Month > 12))
            {
                return HistoryFail("Month must be between 1 and 12", HttpStatusCode.BadRequest);
            }

            var customers = await _billRepo.GetCustomersAsync(userId, filter.CustomerId, null);

            if (customers.Count == 0)
            {
                return EmptyHistory(filter.Year);
            }

            var customerIds = customers.Select(c => c.CustomerId).ToList();

            // The whole year is read even when one month is displayed, so the
            // totals stay correct as the month filter changes.
            var entries = await _billRepo.GetEntriesForYearAsync(customerIds, filter.Year);
            var bills = await _billRepo.GetBillsForYearAsync(customerIds, filter.Year);

            var entriesByCustomer = entries.ToLookup(e => e.CustomerId);
            var billsByCustomer = bills.ToLookup(b => b.CustomerId);

            // Year alone decides the customer list: keep only those belonging to it.
            // A customer has a delivery or a bill in the year, or was created in it.
            // The last clause keeps a customer added today visible before their
            // first entry is recorded.
            customers = customers
                .Where(c => entriesByCustomer[c.CustomerId].Any()
                         || billsByCustomer[c.CustomerId].Any()
                         || (c.CreatedAt != null && c.CreatedAt.Value.Year == filter.Year))
                .ToList();

            if (customers.Count == 0)
            {
                return EmptyHistory(filter.Year);
            }

            var includeDays = filter.IncludeDetail || filter.Month.HasValue;

            var customerDtos = new List<BillHistoryCustomerDto>(customers.Count);
            var yearMonths = new List<BillHistoryMonthDto>(customers.Count * 12);

            var activeCustomers = 0;
            var paidCustomers = 0;
            var unpaidCustomers = 0;

            foreach (var customer in customers)
            {
                var custEntries = entriesByCustomer[customer.CustomerId].ToList();
                var custBills = billsByCustomer[customer.CustomerId].ToList();

                // All twelve months are built every time, ascending, so the UI has
                // a fixed grid and an empty month is explicit rather than missing.
                var months = Enumerable.Range(1, 12)
                    .Select(m => BuildHistoryMonth(
                        filter.Year, m, custEntries, custBills,
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

                IEnumerable<BillHistoryMonthDto> visible = months;

                if (filter.Month.HasValue)
                    visible = visible.Where(m => m.Month == filter.Month.Value);

                if (filter.IsPaid.HasValue)
                    visible = visible.Where(m => m.IsPaid == filter.IsPaid.Value);

                var visibleMonths = visible.ToList();

                // With a paid filter the caller is asking "who has not paid for
                // January", so customers with no matching month are dropped.
                // Without it every customer of the year is returned, which is what
                // the home screen needs.
                if (filter.IsPaid.HasValue && visibleMonths.Count == 0)
                {
                    continue;
                }

                customerDtos.Add(new BillHistoryCustomerDto
                {
                    CustomerId = customer.CustomerId,
                    UserId = customer.UserId,
                    Name = customer.Name,
                    Address = customer.Address,
                    PhotoUrl = customer.PhotoUrl,
                    WhatsappNumber = customer.WhatsappNumber,
                    PhoneNumber = customer.PhoneNumber,
                    Email = customer.Email,
                    CowRate = customer.CowRate,
                    BuffaloRate = customer.BuffaloRate,
                    Months = visibleMonths
                });
            }

            var totalAmount = yearMonths.Sum(m => m.TotalAmount);
            var paidAmount = yearMonths.Sum(m => m.PaidAmount);
            var remainingAmount = yearMonths.Sum(m => m.RemainingAmount);

            return new GeneralResponse<BillHistoryResponseDto>
            {
                Success = true,
                Message = customerDtos.Count == 0
                    ? "No records found for the selected filters"
                    : "History retrieved successfully",
                HttpStatusCode = HttpStatusCode.OK,
                Data = new BillHistoryResponseDto
                {
                    Totals = new BillHistoryTotalsDto
                    {
                        CustomerCount = activeCustomers,
                        PaidCustomerCount = paidCustomers,
                        UnpaidCustomerCount = unpaidCustomers,
                        TotalAmount = totalAmount,
                        PaidAmount = paidAmount,
                        RemainingAmount = remainingAmount
                    },
                    Customers = customerDtos
                }
            };
        }

        private static GeneralResponse<BillHistoryResponseDto> EmptyHistory(int year) =>
            new GeneralResponse<BillHistoryResponseDto>
            {
                Success = true,
                Message = $"No customers found for {year}",
                HttpStatusCode = HttpStatusCode.OK,
                Data = new BillHistoryResponseDto
                {
                    Totals = new BillHistoryTotalsDto(),
                    Customers = new List<BillHistoryCustomerDto>()
                }
            };

        private static BillHistoryMonthDto BuildHistoryMonth(
            int year,
            int month,
            List<Milkentry> entries,
            List<Bill> bills,
            bool includeDays,
            bool includeEmptyDays)
        {
            var monthEntries = entries
                .Where(e => e.Date.Year == year && e.Date.Month == month)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.EntryId)
                .ToList();

            var bill = bills.FirstOrDefault(b => b.Year == year && b.Month == month);

            // Litres and amount always come from the register.
            var totalCowLitre = monthEntries.Sum(e => e.CowLitre ?? 0);
            var totalBuffaloLitre = monthEntries.Sum(e => e.BuffaloLitre ?? 0);
            var totalAmount = monthEntries.Sum(e => e.TotalAmount ?? 0);

            // Payment state comes from the bill. TotalPayable already folds in any
            // PreviousBalance carried forward, so it wins over the entries total.
            var payable = bill?.TotalPayable ?? totalAmount;
            var isPaid = bill?.IsPaymentDone ?? false;

            // Without a payments table there are no instalments to sum, so a month
            // is either settled in full or not settled at all.
            var paidAmount = isPaid ? payable : 0m;
            var remaining = payable - paidAmount;

            string status;
            if (totalAmount <= 0 && bill == null) status = StatusNoActivity;
            else if (isPaid) status = StatusPaid;
            else status = StatusUnpaid;

            var dto = new BillHistoryMonthDto
            {
                Month = month,
                MonthName = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(month),
                TotalCowLitre = totalCowLitre,
                TotalBuffaloLitre = totalBuffaloLitre,
                TotalAmount = totalAmount,
                PaidAmount = paidAmount,
                RemainingAmount = remaining > 0 ? remaining : 0,
                PaymentStatus = status,
                IsPaid = isPaid
            };

            if (!includeDays)
            {
                return dto;
            }

            dto.Days = includeEmptyDays
                ? BuildFullMonthDays(year, month, monthEntries)
                : monthEntries.Select(ToHistoryDay).ToList();

            return dto;
        }

        /// <summary>Every day of the month in order, zero-filled where there was no
        /// delivery. A padded day carries EntryId null. A date holding several
        /// entries emits all of them, so the rows always sum to the month total.</summary>
        private static List<BillHistoryDayDto> BuildFullMonthDays(
            int year, int month, List<Milkentry> monthEntries)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var byDay = monthEntries.ToLookup(e => e.Date.Day);
            var days = new List<BillHistoryDayDto>(daysInMonth);

            for (var day = 1; day <= daysInMonth; day++)
            {
                var dayEntries = byDay[day].OrderBy(e => e.EntryId).ToList();

                if (dayEntries.Count > 0)
                {
                    days.AddRange(dayEntries.Select(ToHistoryDay));
                    continue;
                }

                var date = new DateOnly(year, month, day);
                days.Add(new BillHistoryDayDto
                {
                    EntryId = null,
                    Date = date,
                    DayOfMonth = day,
                    DayName = date.DayOfWeek.ToString()
                });
            }

            return days;
        }

        private static BillHistoryDayDto ToHistoryDay(Milkentry e) => new BillHistoryDayDto
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

        private static GeneralResponse<BillHistoryResponseDto> HistoryFail(
            string message, HttpStatusCode code) =>
            new GeneralResponse<BillHistoryResponseDto>
            {
                Success = false,
                Message = message,
                HttpStatusCode = code
            };






        public async Task<GeneralResponse<BillPreviewDto>> GetBillPreviewAsync(
          int userId, int customerId, int year, int month)
        {
            if (month < 1 || month > 12)
            {
                return PreviewFail("Month must be between 1 and 12", HttpStatusCode.BadRequest);
            }

            if (year < 2000 || year > DateTime.Now.Year + 1)
            {
                return PreviewFail("Invalid year", HttpStatusCode.BadRequest);
            }

            // Ownership and existence in one check.
            var customer = await _billRepo.GetCustomerAsync(userId, customerId);
            if (customer == null)
            {
                return PreviewFail("Customer not found", HttpStatusCode.NotFound);
            }

            // Litres and amount always come from the register, exactly as
            // AddBillAsync computes them, so the preview cannot disagree with what
            // a subsequent POST would save.
            var entries = await _billRepo.GetEntriesForMonthAsync(customerId, year, month);

            var totalCowLitre = entries.Sum(e => e.CowLitre ?? 0);
            var totalBuffaloLitre = entries.Sum(e => e.BuffaloLitre ?? 0);
            var totalAmount = entries.Sum(e => e.TotalAmount ?? 0);

            // Suggested carry-forward. Only the most recent earlier bill is read:
            // its TotalPayable already includes every balance before it, so summing
            // all unsettled bills would count older balances twice.
            var previousBill = await _billRepo.GetLatestEarlierBillAsync(customerId, year, month);

            var suggestedPreviousBalance = previousBill != null && !previousBill.IsPaymentDone
                ? previousBill.TotalPayable ?? 0m
                : 0m;

            var existing = await _billRepo.GetBillAsync(customerId, year, month);

            var preview = new BillPreviewDto
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.Name,
                Month = month,
                MonthName = MonthName(month),
                Year = year,

                EntryCount = entries.Count,
                TotalCowLitre = totalCowLitre,
                TotalBuffaloLitre = totalBuffaloLitre,
                TotalAmount = totalAmount,

                SuggestedPreviousBalance = suggestedPreviousBalance,
                TotalPayable = totalAmount + suggestedPreviousBalance,

                PreviousBillId = previousBill?.BillId,
                PreviousBillMonth = previousBill?.Month,
                PreviousBillYear = previousBill?.Year,

                BillExists = existing != null,
                BillId = existing?.BillId,
                ExistingPreviousBalance = existing?.PreviousBalance,
                ExistingTotalPayable = existing?.TotalPayable,
                ExistingPaymentType = existing?.PaymentType,
                ExistingIsPaymentDone = existing?.IsPaymentDone ?? false
            };

            string message;
            if (existing != null && existing.IsPaymentDone)
            {
                message = $"Bill for {MonthName(month)} {year} is already settled";
            }
            else if (existing != null)
            {
                message = $"A bill for {MonthName(month)} {year} exists and will be recalculated";
            }
            else if (entries.Count == 0 && suggestedPreviousBalance <= 0)
            {
                message = $"No milk entries for {MonthName(month)} {year} and no previous balance";
            }
            else
            {
                message = "Preview generated successfully";
            }

            return new GeneralResponse<BillPreviewDto>
            {
                Success = true,
                Message = message,
                HttpStatusCode = HttpStatusCode.OK,
                Data = preview
            };
        }

        private static GeneralResponse<BillPreviewDto> PreviewFail(string message, HttpStatusCode code) =>
            new GeneralResponse<BillPreviewDto>
            {
                Success = false,
                Message = message,
                HttpStatusCode = code
            };



    }
}
