using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class CoursePromotionView
    {
        public Guid PromotionId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
