using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using Models.Entities;
using Models.Models.Emails;
using Services.Interfaces;
using Settings.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class SendMailService : BaseService, ISendMailService
    {
        private readonly MailSetting mailSettings;

        public SendMailService(ODTutorContext context, IMapper mapper, IOptions<MailSetting> _mailSettings) : base(context, mapper)
        {
            mailSettings = _mailSettings.Value;
        }
        
        public async Task<IActionResult> SendEmailTokenAsync(string email)
        {
            try
            {
                var checkEmail = _context.Users.FirstOrDefault(u => u.Email.Equals(email));

                if (checkEmail == null || checkEmail.EmailConfirmed == true)
                {
                    return new StatusCodeResult(409);
                }
                var tokenEmail = GenerateRandomOTP();
                var checkEmailDefaulr = _context.Users.Any(u => u.Email.Equals(email) && u.EmailConfirmed == false);

                if (checkEmailDefaulr)
                {
                    UserAuthentication userAuthentication = new UserAuthentication
                    {
                        Id = Guid.NewGuid(),
                        UserId = checkEmail.Id,
                        EmailToken = tokenEmail,
                        EmailTokenExpiry = DateTime.UtcNow.AddMinutes(15)
                    };

                    _context.UserAuthentications.Add(userAuthentication);
                    await _context.SaveChangesAsync();
                    try
                    {
                        await SendMail(new MailContent()
                        {
                            To = email,
                            Subject = "[ODTutor] Mã xác thực OTP",
                            Body = "Đây là mã xác thực OTP của bạn" + ".\n Mã này sẽ hết hạn vào " + userAuthentication.EmailTokenExpiry + " GMT +0",
                            OTP = tokenEmail
                        });
                        return new StatusCodeResult(201);
                    }
                    catch (Exception ex)
                    {
                        // Handle the error here, for example log the error message
                        throw new Exception(ex.ToString()); // Return a 500 Internal Server Error status code
                    }
                }
                return new StatusCodeResult(204);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        private string GenerateRandomOTP()
        {
            Random random = new Random();
            int otpValue = random.Next(0, 1000000); // Random số từ 0 đến 999999

            return otpValue.ToString("D6"); // Định dạng để luôn có 6 chữ số
        }

        public async Task<IActionResult> SendMail(MailContent mailContent)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            email.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(mailContent.To));
            email.Subject = mailContent.Subject;

            string projectDirectory = Directory.GetCurrentDirectory();
            string OTPSamplePath = Path.Combine(projectDirectory, "Template", "template.html");
            string htmlContent = System.IO.File.ReadAllText(OTPSamplePath);
            htmlContent = htmlContent.Replace("{Body}", mailContent.Body);
            htmlContent = htmlContent.Replace("{OTP}", mailContent.OTP);
            var builder = new BodyBuilder();
            builder.HtmlBody = htmlContent;
            email.Body = builder.ToMessageBody();

            // dùng SmtpClient của MailKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await email.WriteToAsync(emailsavefile);
                throw;
            }

            smtp.Disconnect(true);
            return new StatusCodeResult(200);
        }
    }
}
