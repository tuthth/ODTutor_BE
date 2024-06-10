using AutoMapper;
using Azure;
using Emgu.CV.Features2D;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class UserService : BaseService, IUserService
    {

        private readonly JWTSetting _jwtSetting;
        public UserService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
            _jwtSetting = _jwtSettings.Value;
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

        // Login By Admin Email
        public async Task<LoginAccountResponse> LoginByAdmin(LoginRequest loginRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Email or Password is empty", "");
                }
                if (!Regex.IsMatch(loginRequest.Password, "^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,12}$"))
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Password must be 8-12 characters, contain at least 1 uppercase letter, 1 lowercase letter, 1 number", "");
                }
                if (loginRequest.Email == _configuration["AdminAccount:EmailAdmin"] 
                    && loginRequest.Password == _configuration["AdminAccount:PasswordAdmin"])
                {
                    var user = _context.Users.FirstOrDefault(u => u.Email == loginRequest.Email);
                    if (user == null)
                    {
                        throw new CrudException(HttpStatusCode.NotFound, "User not found", "");
                    }
                 var response = GenerateJwtTokenAdmin(user);
                 return response;
                }
                else
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Email or Password is incorrect", "");
                }     
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Confirm OTP
        public async Task<IActionResult> ConfirmOTP(string email, string otp)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email.Equals(email));
            if (user == null) return new StatusCodeResult(404); //user not found
            if (user.Banned == true) return new StatusCodeResult(403); //user is banned
            var userAuthentication = _context.UserAuthentications
                .Where(ua => ua.UserId == user.Id && ua.EmailTokenExpiry >= DateTime.UtcNow).OrderByDescending(ua => ua.EmailTokenExpiry).FirstOrDefault();
            if (userAuthentication == null) return new StatusCodeResult(404); //no OTP request found
            if (userAuthentication.EmailToken != otp) return new StatusCodeResult(400); //wrong OTP
            if (userAuthentication.EmailTokenExpiry < DateTime.UtcNow) return new StatusCodeResult(408); //OTP expired
            user.Active = true;
            user.EmailConfirmed = true;
            _context.Users.Update(user);
            var allUserAuthentications = _context.UserAuthentications.Where(ua => ua.UserId == user.Id);
            _context.UserAuthentications.RemoveRange(allUserAuthentications); //remove OTP request after confirmed
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = email,
                Subject = "[ODTutor] Xác thực email thành công",
                Body = "Chúc mừng bạn đã xác thực email thành công. Bạn có thể sử dụng tài khoản của mình để đăng nhập vào hệ thống ODTutor."
            });
            return new StatusCodeResult(200);
        }

        public async Task<IActionResult> ConfirmOTPChangePassword(string email, string oldPassword, string newPassword, string confirmNewPassword, string otp)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email.Equals(email));
            if (user == null) return new StatusCodeResult(404); //user not found
            if (user.Banned == true) return new StatusCodeResult(403); //user is banned
            var userAuthentication = _context.UserAuthentications
                .Where(ua => ua.UserId == user.Id && ua.EmailTokenExpiry >= DateTime.UtcNow).OrderByDescending(ua => ua.EmailTokenExpiry).FirstOrDefault();
            if (userAuthentication == null) return new StatusCodeResult(404); //no OTP request found
            if (userAuthentication.EmailToken != otp) return new StatusCodeResult(400); //wrong OTP
            if (userAuthentication.EmailTokenExpiry < DateTime.UtcNow) return new StatusCodeResult(408); //OTP expired

            if (!_appExtension.VerifyPasswordHash(oldPassword, user.Password)) return new StatusCodeResult(406); //wrong old password
            if (newPassword != confirmNewPassword) return new StatusCodeResult(409); //new password and confirm password not match

            user.Password = _appExtension.CreateHashPassword(newPassword);
            _context.Users.Update(user);
            var allUserAuthentications = _context.UserAuthentications.Where(ua => ua.UserId == user.Id);
            _context.UserAuthentications.RemoveRange(allUserAuthentications); //remove OTP request after confirmed
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = email,
                Subject = "[ODTutor] Thay đổi mật khẩu thành công",
                Body = "Chúc mừng bạn đã thay đổi mật khẩu thành công. Bạn có thể sử dụng mật khẩu mới để đăng nhập vào hệ thống ODTutor."
            });
            return new StatusCodeResult(200);
        }

        public async Task<IActionResult> ConfirmOTPForgotPassword(string email, string newPassword, string confirmNewPassword, string otp)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email.Equals(email));
            if (user == null) return new StatusCodeResult(404); //user not found
            if (user.Banned == true) return new StatusCodeResult(403); //user is banned
            var userAuthentication = _context.UserAuthentications
                .Where(ua => ua.UserId == user.Id && ua.EmailTokenExpiry >= DateTime.UtcNow).OrderByDescending(ua => ua.EmailTokenExpiry).FirstOrDefault();
            if (userAuthentication == null) return new StatusCodeResult(404); //no OTP request found
            if (userAuthentication.EmailToken != otp) return new StatusCodeResult(400); //wrong OTP
            if (userAuthentication.EmailTokenExpiry < DateTime.UtcNow) return new StatusCodeResult(408); //OTP expired

            if (newPassword != confirmNewPassword) return new StatusCodeResult(409); //new password and confirm password not match

            user.Password = _appExtension.CreateHashPassword(newPassword);
            _context.Users.Update(user);
            var allUserAuthentications = _context.UserAuthentications.Where(ua => ua.UserId == user.Id);
            _context.UserAuthentications.RemoveRange(allUserAuthentications); //remove OTP request after confirmed
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = email,
                Subject = "[ODTutor] Thay đổi mật khẩu thành công",
                Body = "Chúc mừng bạn đã thay đổi mật khẩu thành công. Bạn có thể sử dụng mật khẩu mới để đăng nhập vào hệ thống ODTutor."
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
                    Expires = DateTime.UtcNow.AddMinutes(45),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
            }
            else if (moderator != null)
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
                    Expires = DateTime.UtcNow.AddMinutes(45),
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
                    Expires = DateTime.UtcNow.AddMinutes(45),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
            }


            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            LoginAccountResponse response = new LoginAccountResponse
            {
                accessToken = accessToken,
                userId = user.Id,
                role = GetRoleName(tutor, moderator),
                tutorID = tutor?.TutorId,
                moderatorID = moderator?.ModeratorId,
                studentID = student?.StudentId

            };
            return response;
        }

        // Generate Token
        private LoginAccountResponse GenerateJwtTokenAdmin(User user)
        {
            if (user == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "User not found", "");
            }
            var key = Encoding.ASCII.GetBytes(_configuration["AppSettings:SecretKey"]); // Lấy khóa bí mật từ cấu hình
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_jwtSetting.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor();
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, "Admin")
            }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            LoginAccountResponse response = new LoginAccountResponse
            {
                accessToken = accessToken,
                userId = user.Id,
                role = "Admin",
            };
            return response;
        }
        // Get Role 
        private string GetRoleName(Tutor tutor, Moderator moderator)
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
        public async Task<IActionResult> IsUserBanned(Guid userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId && u.Banned == true);
            if (user == null) return new StatusCodeResult(404);
            return new StatusCodeResult(403);
        }
    }
}
