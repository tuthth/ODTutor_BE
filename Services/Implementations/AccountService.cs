using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
        public  AccountService (ODTutorContext odtcontext, IMapper mapper)
        {
            _odtcontext = odtcontext;
            _mapper = mapper;
        }
        
        //RegisterAccount
        public async Task<AccountResponse> createAccount(AccountRegisterRequest accountRegisterRequest)
        {
            try
            {
                if (accountRegisterRequest.FullName == null || accountRegisterRequest.Email == null || accountRegisterRequest.Password == null
                    || accountRegisterRequest.ConfirmPassword == null || accountRegisterRequest.FullName == ""
                    || accountRegisterRequest.Email == "" || accountRegisterRequest.Password == ""
                    || accountRegisterRequest.ConfirmPassword == "")
                    throw new CrudException(HttpStatusCode.BadRequest, "Information is not empty","");
                var s  =_odtcontext.Users.Where( a => a.Email .Equals(accountRegisterRequest.Email.ToUpper().Trim()));
                if (s != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Email is already exist", "");
                }
                var account = _mapper.Map<User>(accountRegisterRequest);
                CreateHashPassword(accountRegisterRequest.Password, out byte[] passwordHash);
                account.Password = Convert.ToBase64String(passwordHash);
                account.Active = true;
                account.EmailConfirmed = false;
                account.Status = 1;
                account.ImageUrl = "https://i.ibb.co/3dLnNK0/defaultavatar-min.png";
                account.Id = new Guid();
                _odtcontext.Users.Add(account);
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
