using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class TutorCertificate
    {
        public Guid TutorCertificateId { get; set; }
        public Guid TutorId { get; set; }
        public string?ImageUrl { get; set; }
        public string CertificateName { get; set; }
        public string CertificateDescription { get; set; }
        public string CertificateFrom { get; set; }
        public string StartYear { get; set; }
        public string EndYear { get; set; }
        public bool IsVerified { get; set; }
        public virtual Tutor? TutorNavigation {  get; set; }
    }
}
