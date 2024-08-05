using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class AccountResponse
    {   
        public Guid StudentId { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public string Password { get; set; }
        public string? Username { get; set; }
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Boolean EmailConfirmed { get; set; }
        public int Status { get; set; }
        public Boolean Active { get; set; }
        public Boolean Banned { get; set; }
        public DateTime? BanExpiredAt { get; set; }
        public Boolean HasBoughtSubscription { get; set; }
        public DateTime? RequestRefreshTime { get; set; }
    }
}
