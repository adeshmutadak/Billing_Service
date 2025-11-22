using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataaa.Response
{
    public class LoginResponse
    {

        public long UserId { get; set; }
       // public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
       // public string Role { get; set; } = null!;
        public string Token { get; set; }  // JWT Token
    }
}
