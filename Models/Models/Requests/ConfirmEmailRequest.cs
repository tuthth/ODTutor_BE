using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class ConfirmEmailRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string OTP { get; set; }
    }
    public class ForgetPasswordRequest : ConfirmEmailRequest
    {
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
    public class ChangePasswordRequest : ConfirmEmailRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
