using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CrudException: Exception
    { 
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public CrudException(HttpStatusCode statusCode,string msg, string errorMessage):base(msg)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}
