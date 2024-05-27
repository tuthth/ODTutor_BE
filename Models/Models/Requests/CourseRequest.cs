using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class CourseRequest
    {
        public Guid TutorId { get; set; }
        public string? Description { get; set; }
        public decimal TotalMoney { get; set; }
        public int TotalSlots { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }
    }
    public class UpdateCourseRequest : CourseRequest
    {
        public Guid CourseId { get; set; }
    }
    public class CourseOutlineRequest
    {
        public Guid CourseId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
    }
    public class UpdateCourseOutlineRequest : CourseOutlineRequest
    {
        public Guid CourseOutlineId { get; set; }
    }
    public class CoursePromotionRequest
    {
        public Guid PromotionId { get; set; }
        public Guid CourseId { get; set; }
    }
    public class CreatePromotion
    {
        public string? PromotionCode { get; set; }
        [Range(0.01, 100, ErrorMessage = "Khuyển mãi phải nằm trong khoảng từ trên 0% đến 100% ")]
        public decimal Percentage { get; set; }
    }
    public class UpdatePromotion : CreatePromotion
    {
        public Guid PromotionId { get; set; }
    }
}
