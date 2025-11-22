using CommonLayer.CommonResponse;
using CommonLayer.PhotoUpload;
using Dataaa.Request;
using Dataaa.Response;
using Dto.Request;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace MilkBilling.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IFileService _fileService;

        public AuthController(IAuthService authService, IFileService fileService)
        {
            _authService = authService;
            _fileService = fileService;
        }


        [HttpPost("register")]
        public async Task<BaseResponse> RegisterUser(RegistrationRequestDto request)
        {
            var response = await _authService.RegisterUser(request);
            return response;
        }


        [HttpPost("login")]
        public async Task<GeneralResponse<LoginResponse>> LoginUser(LogRequest request)
        {
            var response = await _authService.LoginUser(request);
            return response;
        }
    }
}
