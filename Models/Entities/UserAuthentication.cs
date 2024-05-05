using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class UserAuthentication
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? EmailToken { get; set; }
        public string? PhoneOTP { get; set; }
        public DateTime? EmailTokenExpiry { get; set; }
        public DateTime? PhoneOTPExpiry { get; set; }
        public virtual User? UserNavigation { get; set; }
    }
}
