using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorRegisterStep2Response
    {
        public string imageUrl { get; set; }
        public string CertificateDescription { get; set; }
        public string CertifiateForm { get;set; }
        public string CertificateName { get; set; }
        public string StartYear { get; set; }
        public string EndYear { get; set; }
        public bool IsVerified { get; set; }
    }
}
