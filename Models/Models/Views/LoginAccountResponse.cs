using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class LoginAccountResponse
    {
        public string accessToken { get; set; }
        public string role { get; set; }
        public Guid userId { get; set; }
        public Guid ? studentID { get; set; }
        public Guid? tutorID { get;set; }
    }
}
