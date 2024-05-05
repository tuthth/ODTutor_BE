using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class UserAuthentication
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? EmailToken { get; set; }
        public DateTime? EmailTokenExpiry { get; set; }
        public virtual User? UserNavigation { get; set; }
    }
}
