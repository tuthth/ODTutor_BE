using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class UserInFireStore
    {
        public string avatar { get;set; }
        public List<string> blockedUser { get; set; }
        public DateTime LastLogin { get; set;}
        public string name { get; set; } 
        public string userId { get;set; }
    }
}
