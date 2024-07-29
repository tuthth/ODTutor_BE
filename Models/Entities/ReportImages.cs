using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class ReportImages
    {
        public Guid ReportImageId { get; set; }
        public Guid ReportId { get; set; }
        public string ImageURL{ get; set; }

        public virtual Report ReportNavigation { get; set; }
    }
}
