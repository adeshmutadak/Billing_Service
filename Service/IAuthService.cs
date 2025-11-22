using CommonLayer.CommonResponse;
using Dataaa.Request;
using Dataaa.Response;
using Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IAuthService
    {
        Task<BaseResponse> RegisterUser(RegistrationRequestDto request);
        Task<GeneralResponse<LoginResponse>> LoginUser(LogRequest request);
    }
}
