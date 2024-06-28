using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using Models.Entities;
using Models.Enumerables;
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
        private readonly IFirebaseRealtimeDatabaseService _firebaseRealtimeDatabaseService;

        public SendMailService(ODTutorContext context, IMapper mapper, IOptions<MailSetting> _mailSettings, IFirebaseRealtimeDatabaseService firebaseRealtimeDatabaseService) : base(context, mapper)
        {
            mailSettings = _mailSettings.Value;
            _firebaseRealtimeDatabaseService = firebaseRealtimeDatabaseService;
        }
        
        public async Task<IActionResult> SendEmailTokenAsync(string email)
        {
            try
            {
                var checkEmail = _context.Users.FirstOrDefault(u => u.Email.Equals(email));

                if (checkEmail == null)
                {
                    return new StatusCodeResult(409);
                }
                var tokenEmail = _appExtension.GenerateRandomOTP();
                var checkEmailDefaulr = _context.Users.Any(u => u.Email.Equals(email));

                if (checkEmailDefaulr)
                {
                    UserAuthentication userAuthentication = new UserAuthentication
                    {
                        Id = Guid.NewGuid(),
                        UserId = checkEmail.Id,
                        EmailToken = tokenEmail,
                        EmailTokenExpiry = DateTime.UtcNow.AddHours(7).AddMinutes(15)
                    };
                    var notification = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Mã xác thực OTP",
                        Content = "Hãy vào hộp thư email để kiểm tra chi tiết",
                        UserId = checkEmail.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (Int32)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification);
                    await _firebaseRealtimeDatabaseService.SetAsync<Models.Entities.Notification>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                    _context.UserAuthentications.Add(userAuthentication);
                    await _context.SaveChangesAsync();
                    try
                    {
                        await SendMailOTP(new MailContentOTP()
                        {
                            To = email,
                            Subject = "[ODTutor] Mã xác thực OTP",
                            Body = "Đây là mã xác thực OTP của bạn" + ".\n Mã này sẽ hết hạn vào " + userAuthentication.EmailTokenExpiry + " GMT +7",
                            OTP = tokenEmail
                        });   
                    }
                    catch (Exception ex)
                    {
                        
                        throw new Exception(ex.ToString()); 
                    }
                }
                return new StatusCodeResult(201);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }

        

        public async Task<IActionResult> SendMailOTP(MailContentOTP mailContent)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
                email.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
                email.To.Add(MailboxAddress.Parse(mailContent.To));
                email.Subject = mailContent.Subject;

                string projectDirectory = Directory.GetCurrentDirectory();
                string OTPSamplePath = Path.Combine(projectDirectory, "wwwroot", "template.html");
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
                    Console.WriteLine(ex.ToString());

                    // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                    System.IO.Directory.CreateDirectory("mailssave");
                    var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                    await email.WriteToAsync(emailsavefile);
                    throw new Exception(ex.ToString());
                }

                smtp.Disconnect(true);
                return new StatusCodeResult(200);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            
        }
    }
}
