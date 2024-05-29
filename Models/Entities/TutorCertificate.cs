using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class TutorCertificate
    {
        public Guid TutorCertificateId { get; set; }
        public Guid TutorId { get; set; }
        public string ImageUrl { get; set; }

        public virtual Tutor? TutorNavigation {  get; set; }
    }
}
