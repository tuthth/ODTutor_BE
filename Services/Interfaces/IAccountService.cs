using Microsoft.AspNetCore.Mvc;
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
        Task<IActionResult> createAccount(AccountRegisterRequest accountRegisterRequest);
        /*Task<IActionResult> GoogleLoginOrRegister(string idToken);*/
        Task<ActionResult<AccountResponse>> GetStudentInformation(Guid studentID);
        Task<List<UserAccountResponse>> GetAllUser();
        Task<IActionResult> updateUserAccount(UpdateAccountRequest request);
    }
}
