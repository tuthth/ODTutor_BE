using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProcessCrudExceptions
    {
        private readonly RequestDelegate _next;
        public ProcessCrudExceptions(RequestDelegate next)
        {
            _next = next;
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {   
            var message = exception.Message;
            var statusCode = HttpStatusCode.InternalServerError;
            var stackTrace = exception.StackTrace;

            var exceptionResult = new
            {
                error = message,
                stackTrace
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(exceptionResult.ToString());
            /*var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            if (exception is CrudException)
            {
                code = ((CrudException)exception).StatusCode;
            }
            var result = JsonConvert.SerializeObject(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);*/
        }   
    }
}
