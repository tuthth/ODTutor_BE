using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorRegisterCertificateRequest
    {
        public string? ImageUrl { get; set; }
        public string CertificateName { get; set; }
        public string CertificateDescription { get; set; }
        public string CertificateFrom { get; set; }
        public string StartYear { get; set; }
        public string EndYear { get; set; }
    }
}
