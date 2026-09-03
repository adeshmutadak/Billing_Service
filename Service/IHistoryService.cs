using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.CommonResponse;
using Dto.Request;
using Dto.Response;

namespace Service
{
    public interface IHistoryService
    {
        Task<GeneralResponse<HistoryResponseDto>> GetHistoryAsync(int userId, HistoryFilterDto filter);

        Task<GeneralResponse<HistoryFilterOptionsDto>> GetFilterOptionsAsync(int userId);
    }
}
