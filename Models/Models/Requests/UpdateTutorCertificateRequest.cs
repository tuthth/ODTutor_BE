using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class UpdateTutorCertificateRequest
    {
        public Guid TutorCertificateId { get; set; }
        public Guid TutorId { get; set; }
        public string ImageUrl { get; set; }
        public string? CertificateType { get; set; }
        public DateTime? CreateAt { get; set; }
    }
}
