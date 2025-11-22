using CommonLayer.CommonResponse;
using CommonLayer.PhotoUpload;
using CommonLayer.SecurityHelper;
using Dataaa.Request;
using Dataaa.Response;
using Dto.Request;
using MilkBilling.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepo _authRepo;
        private readonly ISecurityHelper _securityHelper;
        private readonly IFileService _fileService;
        public AuthService(IAuthRepo authRepo, ISecurityHelper securityHelper, IFileService fileService)
        {
            _authRepo = authRepo;
            _securityHelper = securityHelper;
            _fileService = fileService;
        }


        public async Task<BaseResponse> RegisterUser(RegistrationRequestDto request)
        {
            var existingUser = await _authRepo.GetUserByEmailAsync(request.Email);
            var existingMobile = await _authRepo.GetUserByMobileAsync(request.Phone);
            if (existingMobile != null)
            {
                return new BaseResponse
                {
                    Message = "User already exists with this Number",
                    Success = false,
                    HttpStatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            if (existingUser != null)
            {
                return new BaseResponse
                {
                    Message = "User already exists with this Email",
                    Success = false,
                    HttpStatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }

            var hashedPassword = _securityHelper.HashPassword(request.Password);

            //string? photoPath = null;

            //if (!string.IsNullOrEmpty(request.ProfilePhoto))
            //{
            //    string folder = @"C:\Users\ZEAL INSTITUTE\Desktop\Dataaa\UploadImages\ProfilePhoto\";
            //    photoPath = await _fileService.SaveBase64ImageAsync(request.ProfilePhoto, folder);

            //    /* if (photoPath == null)
            //         return BadRequest("Failed to save profile image.");*/
            //}


            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Phone = request.Phone,
               
            };

            await _authRepo.AddUserAsync(user);

            return new BaseResponse
            {
                Message = "User registered successfully",
                Success = true,
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                // Data = new { user.Email }
            };
        }

        public async Task<GeneralResponse<LoginResponse>> LoginUser(LogRequest request)
        {


            var user = await _authRepo.GetUserByEmailOrMobAsync(request.EmailOrMobile);



            /*  var user = await _authRepo.GetUserByEmailAsync(request.EmailOrMobile);

              if (user == null)
              {
                  user = await _authRepo.GetUserByMobileAsync(request.EmailOrMobile);
              }*/

            if (user == null || !_securityHelper.VerifyPassword(request.Password, user.PasswordHash))
            {
                return new GeneralResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid email/mobile or password",
                    HttpStatusCode = System.Net.HttpStatusCode.Unauthorized,
                    Data = null
                };
            }

            // Generate JWT token
            var token = _securityHelper.GenerateJwtToken(user);

            var loginResponse = new LoginResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Token = token,
            };

            return new GeneralResponse<LoginResponse>
            {
                Success = true,
                Message = "Login successful",
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                Data = loginResponse
            };
        }


    }
}
