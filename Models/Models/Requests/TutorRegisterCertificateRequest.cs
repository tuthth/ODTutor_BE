using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorRegisterCertificateRequest
    {
        public string CertificateType { get; set; }
        public DateTime CreateAt { get; set; }
        public string CertificateImages {get; set;}
    }
}
