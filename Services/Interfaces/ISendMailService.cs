using Microsoft.AspNetCore.Mvc;
using Models.Models.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISendMailService
    {
        Task<IActionResult> SendMail(MailContent mailContent);
        Task<IActionResult> SendEmailTokenAsync(string email);
    }
}
