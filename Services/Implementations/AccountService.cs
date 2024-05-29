using AutoMapper;
using Google.Apis.Auth;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using Models.Entities;
using Models.Models.Emails;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Interfaces;
using Settings.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly IUserService _userService;
        
        public AccountService(ODTutorContext context, IMapper mapper, IUserService userService) : base(context, mapper)
        {
            _userService = userService;
            
        }

        /*============External Site===========*/

        //RegisterAccount
        public async Task<IActionResult> createAccount(AccountRegisterRequest accountRegisterRequest)
        {
            try
            {
                if (accountRegisterRequest.FullName == null || accountRegisterRequest.Email == null || accountRegisterRequest.Password == null
                    || accountRegisterRequest.ConfirmPassword == null || accountRegisterRequest.FullName == ""
                    || accountRegisterRequest.Email == "" || accountRegisterRequest.Password == ""
                    || accountRegisterRequest.ConfirmPassword == "")
                    return new StatusCodeResult(400);
                var s = _context.Users.FirstOrDefault(a => a.Email.Equals(accountRegisterRequest.Email.ToUpper().Trim()));
                if (s != null)
                {
                    return new StatusCodeResult(409);
                }
                if (accountRegisterRequest.Password != accountRegisterRequest.ConfirmPassword)
                {
                    return new StatusCodeResult(400);
                }
                var account = _mapper.Map<User>(accountRegisterRequest);
                
                account.Password = _appExtension.CreateHashPassword(accountRegisterRequest.Password);
                account.Name = accountRegisterRequest.FullName;
                account.Active = true; // Default is true
                account.EmailConfirmed = false;
                account.DateOfBirth = accountRegisterRequest.DateOfBirth;
                account.PhoneNumber = accountRegisterRequest.PhoneNumber;
                account.Status = 1;
                account.Banned = false;
                account.ImageUrl = "https://firebasestorage.googleapis.com/v0/b/capstone-c0906.appspot.com/o/defaultAva%2FDefaultAva.png?alt=media&token=7f4275d1-05c3-41ca-9ec4-091800bb5895&fbclid=IwZXh0bgNhZW0CMTAAAR1hdvcHNcUznHSgIdEFztHYFX2i1Pij9mEoDLqPBNHaSvbaNJYBdCcqox8_aem_AY0mhUEaiU6HcPLEIXQs3nX8vbFyboGsM08NUkK3knIHfrChNERi9W7lt1cxDwx6-gmGX5jX1yh-14x27xQA1TjF";
                account.Id = new Guid();
                _context.Users.Add(account);
                _context.Students.Add(new Student
                {
                    StudentId = new Guid(),
                    UserId = account.Id
                });
                await _context.SaveChangesAsync();
                //create base wallet for new account
                Wallet wallet = new Wallet
                {
                    WalletId = new Guid(),
                    UserId = account.Id,
                    Amount = 0,
                    AvalaibleAmount = 0,
                    PendingAmount = 0,
                    LastBalanceUpdate = DateTime.UtcNow
                };
                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();
                var accountResponse = _mapper.Map<AccountResponse>(account);
                // Return information of account
                await _appExtension.SendMail(new MailContent()
                {
                    To = account.Email,
                    Subject = "[ODTutor] Thông báo xác thực tài khoản",
                    Body = "Chào mừng bạn đến với ODTutor, vui lòng xác thực tài khoản của bạn để có thể trải nghiệm đầy đủ nhất. "
                });
                return new StatusCodeResult(201);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        // Google Login
        public async Task<LoginAccountResponse> GoogleLoginOrRegister(string idToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings());
                var user = _context.Users.FirstOrDefault(u => u.Email == payload.Email);
                string passwordTemplate = _appExtension.GeneratePassword();
                if (user == null)
                {
                    var registerRequest = new AccountRegisterRequest
                    {
                        FullName = payload.Name,
                        Email = payload.Email,
                        Password = passwordTemplate, // Google không cung cấp mật khẩu, bạn cần xử lý phần này
                        ConfirmPassword = passwordTemplate, // Google không cung cấp mật khẩu, bạn cần xử lý phần này
                        DateOfBirth = DateTime.Now, // Google không cung cấp ngày sinh, bạn cần xử lý phần này
                        PhoneNumber = "00000" // Google không cung cấp số điện thoại, bạn cần xử lý phần này
                    };
                    await createAccount(registerRequest);
                    return await _userService.LoginV2(new LoginRequest
                    {
                        Email = payload.Email,
                        Password = passwordTemplate
                    });
                }
                else
                {
                    LoginRequest loginRequest = new LoginRequest
                    {
                        Email = user.Email,
                        Password = user.Password
                    };
                    return await _userService.LoginV2(loginRequest);
                }
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Get Student Information
        public async Task<ActionResult<AccountResponse>> GetStudentInformation(Guid UserID)
        {
            try
            {
                var response = new AccountResponse();
                var userInfo = _context.Users
                    .FirstOrDefault(s => s.Id.Equals(UserID));
                if (userInfo == null) return new StatusCodeResult(404);

                    response.Status = userInfo.Status;
                    response.Email = userInfo.Email;
                    response.FullName = userInfo.Name;
                    response.ImageUrl = userInfo.ImageUrl;
                    response.PhoneNumber = userInfo.PhoneNumber;
                    response.DateOfBirth = userInfo.DateOfBirth;
                    response.EmailConfirmed = userInfo.EmailConfirmed;
                    response.Active = userInfo.Active;
                    response.Banned = userInfo.Banned;
                    response.Status = userInfo.Status;
                    response.EmailConfirmed = userInfo.EmailConfirmed;
                    return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Update User Account
        public async Task<IActionResult> updateUserAccount(UpdateAccountRequest request)
        {
            try
            {
                User user = FindUserById(request.Id);
                if (user == null)
                {
                    return new StatusCodeResult(404);
                }
                user.Name = request.FullName;
                user.Username = request.Username;
                user.ImageUrl = request.ImageUrl;
                user.PhoneNumber = request.PhoneNumber;
                user.DateOfBirth = request.DateOfBirth ?? DateTime.MinValue;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                await _appExtension.SendMail(new MailContent()
                {
                    To = user.Email,
                    Subject = "[ODTutor] Thông báo cập nhật thông tin tài khoản",
                    Body = "Thông tin tài khoản của bạn đã được cập nhật thành công"
                });
                return new StatusCodeResult(200);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Get All User 
        public async Task<List<UserAccountResponse>> GetAllUser()
        {
            try
            {
                List<UserAccountResponse> response = new List<UserAccountResponse>();
                response = _context.Users.Select(s => new UserAccountResponse
                {
                    UserID = s.Id,
                    Email = s.Email,
                    FullName = s.Name,
                    ImageUrl = s.ImageUrl,
                    PhoneNumber = s.PhoneNumber,
                    DateOfBirth = s.DateOfBirth,
                    EmailConfirmed = s.EmailConfirmed,
                    Active = s.Active,
                    Banned = s.Banned,
                    Status = s.Status
                }).ToList();
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

        /*========== Internal Site ==========*/

        // Find User By ID 
        private User FindUserById(Guid UserID)
        {
            return _context.Users.FirstOrDefault(s => s.Id.Equals(UserID));
        }


    }
}
