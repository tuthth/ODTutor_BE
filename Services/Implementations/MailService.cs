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
            var checkEmail = _context.Users.Any(u => u.Email.Equals(email) && u.EmailConfirmed == true);

            if (checkEmail)
            {
                return new StatusCodeResult(409);
            }
            var tokenEmail = GenerateRandomOTP();
            var checkEmailDefaulr = _context.Users.Any(u => u.Email.Equals(email) && u.EmailConfirmed == false);
            if (checkEmailDefaulr)
            {
                var queryUserAuthentication = _context.Users.Include(u => u.UserAuthenticationNavigation).FirstOrDefault(u => u.Email.Equals(email));
                queryUserAuthentication.UserAuthenticationNavigation.EmailToken = tokenEmail;
                queryUserAuthentication.UserAuthenticationNavigation.EmailTokenExpiry = DateTime.Now;
                _context.UserAuthentications.Update(queryUserAuthentication.UserAuthenticationNavigation);
                await _context.SaveChangesAsync();
                await SendMail(new MailContent()
                {
                    To = email,
                    Subject = "Mã xác thực Otp",
                    Body = "Đây là mã xác thực Otp của bạn: " + tokenEmail
                });
                return new StatusCodeResult(200);
            }
            User user = new User
            {
                Email = email,
                Active = false,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            UserAuthentication userAuthentication = new UserAuthentication
            {
                UserId = user.Id,
                EmailToken = tokenEmail,
                EmailTokenExpiry = DateTime.UtcNow
            };
            _context.UserAuthentications.Add(userAuthentication);
            await _context.SaveChangesAsync();
            await SendMail(new MailContent()
            {
                To = email,
                Subject = "Mã xác thực Otp",
                Body = "Đây là mã xác thực Otp của bạn: " + tokenEmail
            });
            return new StatusCodeResult(200);
        }
        private string GenerateRandomOTP()
        {
            Random random = new Random();
            int otpValue = 0;

            // Loop để đảm bảo rằng mã OTP không bắt đầu bằng số 0
            while (otpValue < 100000)
            {
                otpValue = random.Next(100000, 999999);
            }

            return otpValue.ToString();
        }


        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await SendMail(new MailContent()
            {
                To = email,
                Subject = subject,
                Body = htmlMessage
            });
        }

        public async Task SendMail(MailContent mailContent)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            email.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(mailContent.To));
            email.Subject = mailContent.Subject;


            var builder = new BodyBuilder();
            builder.HtmlBody = mailContent.Body;
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
                // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await email.WriteToAsync(emailsavefile);

            }

            smtp.Disconnect(true);
        }
    }
}
