using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Guid GetUserId(HttpContext httpContext);
        Task<IActionResult> Login(LoginRequest loginRequest, int role);
        Task<IActionResult> GenerateJwtToken(User user, int role);
        Task<IActionResult> ConfirmOTP(string email, string otp);
    }
}
