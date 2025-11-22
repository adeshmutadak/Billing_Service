using Microsoft.EntityFrameworkCore;
using MilkBilling.Data;
using MilkBilling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class AuthRepo : IAuthRepo
    {

        private readonly AppDbContext _context;

        public AuthRepo(AppDbContext dbContext)
        {
            _context = dbContext; 
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }



        public async Task<User> GetUserByEmailOrMobAsync(string mobileOrPhone)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == mobileOrPhone || x.Phone== mobileOrPhone);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByMobileAsync(string mobile)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Phone == mobile);
        }
    }
}
