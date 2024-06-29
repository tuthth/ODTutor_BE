using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class UserFireStoreReponse
    {   

        public string Avatar { get;set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Guid UserId { get; set; }
        public List<Guid> blockedUser { get; set; } 
    }
}
