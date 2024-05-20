using Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class UpdateAccountRequest
    {
        public string FullName { get; set;  }
        public string? Username { get; set; }
        public string? ImageUrl { get; set; }
        [DateOfBirthValidation(ErrorMessage ="Ngày tháng năm sinh không được bằng thời gian hiện tại")]
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
    }
}
