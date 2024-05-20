using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class UpdateUserAccountRequest
    {  
        public Guid userID { get;set;}
        public DateTime? dateOfBirth { get; set; }
        public string PhoneNumber {get; set;}
        public string userName { get; set; }
    }
}
