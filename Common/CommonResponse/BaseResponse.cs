using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.CommonResponse
{
    public class BaseResponse
    {
        public List<string > Errors { get; set; }
        public List<string> Warnings { get; set;}
        public bool Success { get; set; }
        public string Message { get; set; }
        public long TimeInMillis { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }

        
        public BaseResponse()
        {
            Errors= new List<string>();
            Warnings= new List<string>();
            HttpStatusCode = HttpStatusCode;
        }

        public override string ToString()
        {
            return string.Format("[Response]:Errors : {0} , Warning : {1} , Message : {2} , Success: {3} , TimeInMillis : {4},HttpStatusCode: {5}",
                string.Join(", ", Errors.ToArray()),
                string.Join(", ", Warnings.ToArray()),
                Message,
                Success,
                TimeInMillis,
                HttpStatusCode
                );
        }
    }
}
