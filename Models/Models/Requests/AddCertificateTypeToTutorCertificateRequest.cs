using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class CertificateTypeToTutorCertificateRequest
    {
        public Guid CertificateTypeId { get; set; }
        public Guid TutorCertificateId { get; set; }
    }
}
