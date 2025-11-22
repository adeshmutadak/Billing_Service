using MilkBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IAuthRepo
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByMobileAsync(string mobile);
        Task<User> GetUserByEmailOrMobAsync(string mobileOrPhone);

        //Task<User> GetUserByRole(string role, string mobOrEmail);
        Task AddUserAsync(User user);
    }
}
