using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorRegisterReponse
    {
        public Guid? TutorId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? ImageUrl { get; set; }
        public string IdentityNumber { get; set; }
        public string Level { get; set; }
        public decimal PricePerHour { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public List<string>ImagesCertificateUrl{get; set;}
        public List<string> Subjects{ get; set;}
    }
}
