using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.CommonResponse
{
    public class GeneralResponse<T>:BaseResponse
    {
       
        public T Data { get; set; }

      
    }
}
