using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Requests;
using Services.Interfaces;
using Settings.JWT;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class UserService : BaseService, IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly JWTSetting _jwtSetting;
        public UserService(ODTutorContext context, IMapper mapper, IOptions<JWTSetting> options) : base(context, mapper)
        {
            _jwtSetting = options.Value;
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
        public async Task<IActionResult> Login(LoginRequest loginRequest, int role)
        {
            var user = _context.Users.FirstOrDefault(u => (u.Email == loginRequest.Email || u.Username == loginRequest.Username) && u.Password.Equals(loginRequest.Password));
            if(user == null) return new StatusCodeResult(404);
            if(user.Active == false) return new StatusCodeResult(409); //user is not active in system
            if(user.Banned == true) return new StatusCodeResult(403); //user is banned

            if (user != null)
            {
                return await GenerateJwtToken(user, role);
            }
            else
            {
                return new StatusCodeResult(400);
            }
        }
        public async Task<IActionResult> GenerateJwtToken(User user, int role)
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
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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
        }
        public async Task<IActionResult> ConfirmOTP(string email, string otp)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return new StatusCodeResult(404); //user not found
            if(user.Active == true) return new StatusCodeResult(409); //user is active in system
            if(user.Banned == true) return new StatusCodeResult(403); //user is banned
            var userAuthentication = _context.UserAuthentications.FirstOrDefault(ua => ua.UserId == user.Id);
            if (userAuthentication == null) return new StatusCodeResult(404); //no OTP request found
            if (userAuthentication.EmailToken != otp) return new StatusCodeResult(400); //wrong OTP
            if (userAuthentication.EmailTokenExpiry < DateTime.UtcNow) return new StatusCodeResult(408); //OTP expired
            user.Active = true;
            user.EmailConfirmed = true;
            _context.Users.Update(user);
            _context.UserAuthentications.Remove(userAuthentication); //remove OTP request after confirmed
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> RemoveExpiredOTP()
        {
            var expiredOTP = _context.UserAuthentications.Where(ua => ua.EmailTokenExpiry < DateTime.UtcNow);
            if(expiredOTP.Count() == 0) return new StatusCodeResult(404); //no expired OTP found
            _context.UserAuthentications.RemoveRange(expiredOTP);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
    }
}
