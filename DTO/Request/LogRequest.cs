using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataaa.Request
{
    public class LogRequest
    {
        public string EmailOrMobile { get; set; }   // can be either Email or Mobile
       // public string Role { get; set; } = null!;
        public string Password { get; set; }
    }
}
