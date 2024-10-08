﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Promotion
    {
        public Guid PromotionId { get; set; }
        public string? PromotionCode { get; set; }
        public Guid TutorId { get; set; }
        public decimal Percentage {  get; set; }
        public DateTime CreatedAt {  get; set; }
        public virtual Tutor? TutorNavigation { get; set; }
        public virtual ICollection<CoursePromotion>? CoursePromotionsNavigation { get; set; }
    }
}
