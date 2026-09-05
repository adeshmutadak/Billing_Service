using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.CommonResponse;
using Dto.Request;

namespace Service
{
    public interface IBillService
    {
        /// <summary>Generates the monthly bill for a customer. Litres and amount
        /// are calculated from milkentries; PreviousBalance comes from the caller.
        /// Recalculates an existing unpaid bill for the same period instead of
        /// creating a duplicate.</summary>
        Task<GeneralResponse<BillResponseDto>> AddBillAsync(int userId, AddBillRequestDto dto);

        Task<GeneralResponse<BillHistoryResponseDto>> GetHistoryAsync(
              int userId, BillHistoryFilterDto filter);

        Task<GeneralResponse<BillPreviewDto>> GetBillPreviewAsync(
            int userId, int customerId, int year, int month);
    }
}
