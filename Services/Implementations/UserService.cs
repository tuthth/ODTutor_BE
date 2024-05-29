using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Emails;
using Models.Models.Requests;
using Models.Models.Views;
using NuGet.Common;
using Services.Interfaces;
using Settings.JWT;
using Settings.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class UserService : BaseService, IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly JWTSetting _jwtSetting;
        private readonly MailSetting mailSettings;
        public UserService(ODTutorContext context, IConfiguration configuration, IMapper mapper, IOptions<JWTSetting> options, IOptions<MailSetting> mailOptions) : base(context, mapper)
        {
            _jwtSetting = options.Value;
            mailSettings = mailOptions.Value;
            _configuration = configuration;
        }

        public Guid GetUserId(HttpContext httpContext)
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                return Guid.Parse(userIdClaim.Value);
            }
            return Guid.Empty;
        }
        // Login Version 1
        /* public async Task<IActionResult> Login(LoginRequest loginRequest, int role)
         {
             var user = _context.Users.FirstOrDefault(u => (u.Email == loginRequest.Email || u.Username == loginRequest.Username) && u.Password.Equals(loginRequest.Password));
             if (user == null) return new StatusCodeResult(404);
             if (user.Active == false) return new StatusCodeResult(409); //user is not active in system
             if (user.Banned == true) return new StatusCodeResult(403); //user is banned

             if (user != null)
             {
                 return await GenerateJwtToken(user, role);
             }
             else
             {
                 return new StatusCodeResult(400);
             }
         }*/

        // Login Version 2
        public async Task<LoginAccountResponse> LoginV2(LoginRequest loginRequest)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => (u.Email == loginRequest.Email));
                if (user == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "User not found", "");
                }
                if (user.Active == false)
                {
                    throw new CrudException(HttpStatusCode.Conflict, "User is not active in system", "");
                }
                if (user.Banned == true)
                {
                    throw new CrudException(HttpStatusCode.Forbidden, "User is banned", "");
                }
                if (!_appExtension.VerifyPasswordHash(loginRequest.Password.Trim(), user.Password))
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Incorrect Password!", "");
                }
                var response = GenerateJwtTokenV2(user);
                response.studentID = user.StudentNavigation.StudentId;
                return response;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }
        // V1 - tạm thời khóa
        /*public async Task<IActionResult> GenerateJwtToken(User user, int role)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["AppSettings:SecretKey"]); // Lấy khóa bí mật từ cấu hình
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_jwtSetting.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor();
            if (role == (Int32)UserRoles.Student)
            {
                var student = _context.Students.FirstOrDefault(s => s.UserId == user.Id);
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                {
                new Claim("UserId", user.Id.ToString()),
                new Claim("StudentId", student.StudentId.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, role.ToString())
            }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
            }
            if (role == (Int32)UserRoles.Tutor)
            {
                var tutor = _context.Tutors.FirstOrDefault(t => t.UserId == user.Id);
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim("UserId", user.Id.ToString()),
                new Claim("TutorId", tutor.TutorId.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, role.ToString())
            }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
            }


            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            return new JsonResult(new
            {
                AccessToken = accessToken
            });
        }*/

        // Confirm OTP
        public async Task<IActionResult> ConfirmOTP(string email, string otp)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return new StatusCodeResult(404); //user not found
            if (user.Active == true) return new StatusCodeResult(409); //user is active in system
            if (user.Banned == true) return new StatusCodeResult(403); //user is banned
            var userAuthentication = _context.UserAuthentications.FirstOrDefault(ua => ((ua.UserId == user.Id) && ua.EmailTokenExpiry.Value.Date < DateTime.UtcNow.Date));
            if (userAuthentication == null) return new StatusCodeResult(404); //no OTP request found
            if (userAuthentication.EmailToken != otp) return new StatusCodeResult(400); //wrong OTP
            if (userAuthentication.EmailTokenExpiry < DateTime.UtcNow) return new StatusCodeResult(408); //OTP expired
            user.Active = true;
            user.EmailConfirmed = true;
            _context.Users.Update(user);
            _context.UserAuthentications.Remove(userAuthentication); //remove OTP request after confirmed
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = email,
                Subject = "[ODTutor] Xác thực email thành công",
                Body = "Chúc mừng bạn đã xác thực email thành công. Bạn có thể sử dụng tài khoản của mình để đăng nhập vào hệ thống ODTutor."
            });
            return new StatusCodeResult(200);
        }

        //Remove Expired
        public async Task<IActionResult> RemoveExpiredOTP()
        {
            var expiredOTP = _context.UserAuthentications.Where(ua => ua.EmailTokenExpiry < DateTime.UtcNow);
            if (expiredOTP.Count() == 0) return new StatusCodeResult(404); //no expired OTP found
            _context.UserAuthentications.RemoveRange(expiredOTP);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }

        // Ban Account
        public async Task<IActionResult> BanAccount(Guid userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return new StatusCodeResult(404); //user not found
            if (user.Banned == true) return new StatusCodeResult(409); //user is already banned
            user.Banned = true;
            user.BanExpiredAt = DateTime.UtcNow.AddYears(30); //ban for 24 hours
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            try
            {
                await _appExtension.SendMail(new MailContent()
                {
                    To = user.Email,
                    Subject = "[ODTutor] Thông báo đình chỉ tài khoản",
                    Body = "Tài khoản của bạn bị đình chỉ do vi phạm chính sách sử dụng của ODTutor. Để mở khóa trước thời hạn, vui lòng liên hệ lại email này. \nTài khoản sẽ được tự động mở khóa vào lúc " + user.BanExpiredAt + " GMT+0",

                });
                return new StatusCodeResult(200);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new StatusCodeResult(500);
            }
        }

        // Send Mail Confirm Banned
        




        /*========== Internal Site ==========*/

        // Find Tutor Id Based on User Id
        public Tutor findTutor(Guid userId)
        {
            return _context.Tutors.FirstOrDefault(t => t.UserId == userId);
        }

        // Find Moderator Id Based on User Id
        private Moderator findModerator(Guid userId)
        {
            return _context.Moderators.FirstOrDefault(m => m.UserId == userId);
        }

        // Generate Token
        private LoginAccountResponse GenerateJwtTokenV2(User user)
        {
            if (user == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "User not found", "");
            }
            var key = Encoding.ASCII.GetBytes(_configuration["AppSettings:SecretKey"]); // Lấy khóa bí mật từ cấu hình
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_jwtSetting.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor();
            var tutor = findTutor(user.Id);
            var moderator = findModerator(user.Id);
            var student = _context.Students.FirstOrDefault(s => s.UserId == user.Id);
            if (tutor == null && moderator == null)
            {
                var studentInfo = _context.Students.FirstOrDefault(s => s.UserId == user.Id);
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                {
                new Claim("UserId", user.Id.ToString()),
                new Claim("StudentId", student.StudentId.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role,"Student")
            }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
            }
            else if ( moderator != null)
            {
                var moderatorInfo = _context.Moderators.FirstOrDefault(t => t.UserId == user.Id);
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim("UserId", user.Id.ToString()),
                new Claim("ModeratorId", moderator.ModeratorId.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, "Moderator")
            }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
            }
            else
            {
                var tutorInfo = _context.Tutors.FirstOrDefault(t => t.UserId == user.Id);
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim("UserId", user.Id.ToString()),
                new Claim("TutorId", tutor.TutorId.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, "Tutor")
            }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
            }


            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            LoginAccountResponse response = new LoginAccountResponse
            {
                accessToken = accessToken,
                userId = user.Id,
                role = GetRoleName(tutor,moderator),
                tutorID = tutor?.TutorId,
                moderatorID = moderator?.ModeratorId,
                studentID = student?.StudentId

            };
            return response;
        }
        // Get Role 
        private string GetRoleName( Tutor tutor, Moderator moderator)
        {
            if (tutor != null)
            {
                return "Tutor";
            }
            else if (moderator != null)
            {
                return "Moderator";
            }
            else
            {
                return "Student";
            }
        }
    }
}
