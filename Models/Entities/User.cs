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
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Boolean EmailConfirmed { get; set; } = false;
        public string? Username { get; set; }
        public string? ImageUrl { get; set; }
        public int Status { get; set; }
        public Boolean Active { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Boolean IsPremium { get; set; }
        public Boolean Banned { get; set; }
        public DateTime BanExpiredAt { get; set; }
        public virtual UserAuthentication? UserAuthenticationNavigation { get; set; }
        public virtual ICollection<UserBlock>? CreateUserBlockNavigation {  get; set; }
        public virtual ICollection<UserBlock>? TargetUserBlockNavigation { get; set; }
        public virtual ICollection<UserFollow>? CreateUserFollowNavigation {  get; set; }
        public virtual ICollection<UserFollow>? TargetUserFollowNavigation { get; set; }
        public virtual ICollection<Report>? SenderUserReportNavigation {  get; set; }
        public virtual Student? StudentNavigation {  get; set; }
        public virtual Tutor? TutorNavigation { get; set; }

        public virtual Wallet? WalletNavigation { get; set; }
    }
}
