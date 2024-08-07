using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class CertificateTypeRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
    public class UpdateCertificateTypeRequest: CertificateTypeRequest
    {
        public Guid CertificateTypeId { get; set; }
    }
}
