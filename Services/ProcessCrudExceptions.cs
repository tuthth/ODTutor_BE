using Microsoft.AspNetCore.Http;
using Models.Models.CrudException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
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

            var statusCode = HttpStatusCode.InternalServerError;
            var traceId = context.TraceIdentifier;
            var errors = new Dictionary<string, string[]>();

            // Handle Different Exception Types
            
            if(exception is CrudException crudException)
            {
                statusCode = crudException.StatusCode;
                errors.Add("ServerError", new string[] {crudException.Message});
            }
            else
            {
                errors.Add("ServerError", new string[] {exception.Message});
            }

            var exceptionResponse  = new CrudExceptionResponse
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = statusCode == HttpStatusCode.BadRequest? "One or more validation errors occurred.":
                        statusCode == HttpStatusCode.Unauthorized? "Unauthorized access" : "Internal Server Error",
                Status = (int)statusCode,
                TraceId = traceId,
                Errors = errors
            };

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var jsonResponse = JsonSerializer.Serialize(exceptionResponse, jsonOptions);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(jsonResponse);
        }   

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            }
    }
}
