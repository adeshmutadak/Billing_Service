
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MilkBilling.Models;


namespace CommonLayer.SecurityHelper
{
    public class SecurityHelper : ISecurityHelper
    {
        private readonly IConfiguration _config;

        public SecurityHelper(IConfiguration config)
        {
            _config = config;
        }
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        public string GenerateJwtToken(User user)
        {
            // minimal JWT example
            var claims = new[]
             {
                new Claim("UserId", user.UserId.ToString()),  // custom claim
               // new Claim(ClaimTypes.Email, user.Email),
              //  new Claim(ClaimTypes.Role, user.Role)         // optional but useful
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
