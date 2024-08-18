using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class CertificateTypeView
    {
        public Guid CertificateTypeId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
