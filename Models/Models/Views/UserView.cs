using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class UserView
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Boolean EmailConfirmed { get; set; } = false;
        public string? Username { get; set; }
        public string? ImageUrl { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Status { get; set; }
        public Boolean Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public Boolean IsPremium { get; set; }
        public Boolean Banned { get; set; }
        public DateTime? BanExpiredAt { get; set; }
        public string ? UserRole { get; set; }
    }
}
