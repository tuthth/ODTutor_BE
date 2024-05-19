using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public class AccountService : IAccountService
    {
        private readonly ODTutorContext _odtcontext;
        private readonly IMapper _mapper;
        public AccountService(ODTutorContext odtcontext, IMapper mapper)
        {
            _odtcontext = odtcontext;
            _mapper = mapper;
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
                var s = _odtcontext.Users.FirstOrDefault(a => a.Email.Equals(accountRegisterRequest.Email.ToUpper().Trim()));
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
                account.Active = false; // Default is false
                account.EmailConfirmed = false;
                account.DateOfBirth = null;
                account.PhoneNumber = "";
                account.Status = 1;
                account.Banned = false;
                account.ImageUrl = "https://firebasestorage.googleapis.com/v0/b/capstone-c0906.appspot.com/o/defaultAva%2FDefaultAva.png?alt=media&token=7f4275d1-05c3-41ca-9ec4-091800bb5895&fbclid=IwZXh0bgNhZW0CMTAAAR1hdvcHNcUznHSgIdEFztHYFX2i1Pij9mEoDLqPBNHaSvbaNJYBdCcqox8_aem_AY0mhUEaiU6HcPLEIXQs3nX8vbFyboGsM08NUkK3knIHfrChNERi9W7lt1cxDwx6-gmGX5jX1yh-14x27xQA1TjF";
                account.Id = new Guid();
                _odtcontext.Users.Add(account);
                _odtcontext.Students.Add(new Student
                {
                    StudentId = new Guid(),
                    UserId = account.Id
                });
                await _odtcontext.SaveChangesAsync();
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

        // Get Student Information
        public async Task<AccountResponse> GetStudentInformation(Guid UserID)
        {
            try
            {
                var response = new AccountResponse();
                var userInfo = _odtcontext.Users
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
        // Create Hash Password
        private void CreateHashPassword(string password, out byte[] passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
