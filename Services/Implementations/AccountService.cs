﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using Services.Interfaces;
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

        //RegisterAccount
        //cai nay dc moi student a` Chuong
        public async Task<AccountResponse> createAccount(AccountRegisterRequest accountRegisterRequest)
        {
            try
            {
                if (accountRegisterRequest.FullName == null || accountRegisterRequest.Email == null || accountRegisterRequest.Password == null
                    || accountRegisterRequest.ConfirmPassword == null || accountRegisterRequest.FullName == ""
                    || accountRegisterRequest.Email == "" || accountRegisterRequest.Password == ""
                    || accountRegisterRequest.ConfirmPassword == "")
                    throw new CrudException(HttpStatusCode.BadRequest, "Information is not empty", "");
                var s = _context.Users.FirstOrDefault(a => a.Email.Equals(accountRegisterRequest.Email.ToUpper().Trim()));
                if (s != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Email is already exist", "");
                }
                if (accountRegisterRequest.Password != accountRegisterRequest.ConfirmPassword)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Password and Confirm Password are not the same", "");
                }
                var account = _mapper.Map<User>(accountRegisterRequest);
                CreateHashPassword(accountRegisterRequest.Password, out byte[] passwordHash);
                account.Password = Convert.ToBase64String(passwordHash);
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
                return accountResponse;
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

        public async Task<IActionResult> GoogleLoginOrRegister(string idToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings());

                var user = _context.Users.FirstOrDefault(u => u.Email == payload.Email);
                if (user == null)
                {
                    var registerRequest = new AccountRegisterRequest
                    {
                        FullName = payload.Name,
                        Email = payload.Email,
                        Password = "", // Google không cung cấp mật khẩu, bạn cần xử lý phần này
                        ConfirmPassword = "", // Google không cung cấp mật khẩu, bạn cần xử lý phần này
                        DateOfBirth = DateTime.Now, // Google không cung cấp ngày sinh, bạn cần xử lý phần này
                        PhoneNumber = "" // Google không cung cấp số điện thoại, bạn cần xử lý phần này
                    };
                    return (IActionResult)await createAccount(registerRequest);
                }
                else
                {
                    var loginRequest = new LoginRequest
                    {
                        Email = user.Email,
                        Password = user.Password
                    };
                    return await _userService.Login(loginRequest, 1); // Giả sử rằng role là 1
                }
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Get Student Information
        public async Task<AccountResponse> GetStudentInformation(Guid UserID)
        {
            try
            {
                var response = new AccountResponse();
                var userInfo = _context.Users
                    .FirstOrDefault(s => s.Id.Equals(UserID));

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
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }
        /*public async Task<> getStudentInformation (Guid studentId)*/

        // Update User Account
        public async Task<UserAccountResponse> updateUserAccount(Guid UserID, UpdateAccountRequest request)
        {
            try
            {
                User user = FindUserById(UserID);
                if (user == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "User not found", "");
                }
                user.Name = request.FullName;
                user.Username = request.Username;
                user.ImageUrl = request.ImageUrl;
                user.PhoneNumber = request.PhoneNumber;
                user.DateOfBirth = request.DateOfBirth ?? DateTime.MinValue;
                UserAccountResponse response = _mapper.Map<UserAccountResponse>(user);
                response.UserID = user.Id;
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

        // Create Hash Password
        private void CreateHashPassword(string password, out byte[] passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // Find User By ID 
        private User FindUserById(Guid UserID)
        {
            return _context.Users.FirstOrDefault(s => s.Id.Equals(UserID));
        }
    }
}
