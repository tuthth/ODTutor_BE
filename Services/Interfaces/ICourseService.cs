using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICourseService
    {
        Task<IActionResult> CreateCourse(CourseRequest courseRequest);
        Task<IActionResult> UpdateCourse(UpdateCourseRequest courseRequest);
        Task<IActionResult> DeleteCourse(Guid id);
        Task<IActionResult> CreateCourseOutline(CourseOutlineRequest courseOutlineRequest);
        Task<IActionResult> UpdateCourseOutline(UpdateCourseOutlineRequest courseOutlineRequest);
        Task<IActionResult> DeleteCourseOutline(Guid id);
        Task<IActionResult> CreateCoursePromotion(CoursePromotionRequest coursePromotionRequest);
        Task<IActionResult> DeleteCoursePromotion(CoursePromotionRequest coursePromotionRequest);
        Task<IActionResult> UpdateCoursePromotion(CoursePromotionRequest coursePromotionRequest);
        Task<IActionResult> CreatePromotion(CreatePromotion createPromotion);
        Task<IActionResult> UpdatePromotion(UpdatePromotion updatePromotion);
        Task<IActionResult> DeletePromotion(Guid id);
        Task<IActionResult> CreateCourseSlot(CourseSlotRequest request);
        Task<IActionResult> UpdateCourseSlot(UpdateCourseSlotRequest request);
        Task<IActionResult> DeleteCourseSlot(Guid id);
        Task<IActionResult> SwapSlotNumber(CourseSlotSwapRequest request);
        Task<ActionResult<List<Course>>> GetAllCourses();
        Task<ActionResult<Course>> GetCourse(Guid id);
        Task<ActionResult<List<CourseOutline>>> GetAllCourseOutlines();
        Task<ActionResult<CourseOutline>> GetCourseOutline(Guid id);
        Task<ActionResult<List<CoursePromotion>>> GetAllCoursePromotions();
        Task<ActionResult<CoursePromotion>> GetCoursePromotion(Guid id);
        Task<ActionResult<List<Promotion>>> GetAllPromotions();
        Task<ActionResult<List<Promotion>>> GetPromotionsByTutorId(Guid tutorId);
        Task<ActionResult<Promotion>> GetPromotion(Guid id);
        Task<ActionResult<List<CourseSlot>>> GetAllCourseSlots();
        Task<ActionResult<CourseSlot>> GetCourseSlot(Guid id);
        Task<ActionResult<List<CourseSlot>>> GetCourseSlotsByCourseId(Guid id);
    }
}
