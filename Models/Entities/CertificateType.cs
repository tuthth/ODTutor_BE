using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class CertificateType
    {
        public Guid CertificateTypeId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<TutorCertificate>? TutorCertificatesNavigation { get; set; }
    }
}
