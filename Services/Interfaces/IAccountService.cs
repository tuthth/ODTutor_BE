﻿using Microsoft.AspNetCore.Mvc;
using Models.Models.Requests;
using Models.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponse> createAccount(AccountRegisterRequest accountRegisterRequest);
        Task<IActionResult> GoogleLoginOrRegister(string idToken);
        Task<AccountResponse> GetStudentInformation(Guid studentID);
    }
}
