using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class CoursePromotion
    {
        public Guid PromotionId { get; set; }
        public Guid CourseId {  get; set; }
        public DateTime CreatedAt {  get; set; }

        public virtual Course? CourseNavigation { get; set; }
        public virtual ICollection<Promotion>? PromotionsNavigation {  get; set; }
    }
}
