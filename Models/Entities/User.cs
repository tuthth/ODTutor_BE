using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Boolean EmailConfirmed { get; set; } = false;
        public string? Phone { get; set; }
        public Boolean PhoneConfirmed { get; set; } = false;
        public int? RoleId { get; set; }
        public Boolean Active { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual UserAuthentication? UserAuthenticationNavigation { get; set; }
        public virtual Wallet? WalletNavigation { get; set; }
    }
}
