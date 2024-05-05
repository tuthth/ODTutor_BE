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
        Task SendMail(MailContent mailContent);

        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task<IActionResult> SendEmailTokenAsync(string email);
    }
}
