using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Requests;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class CourseService : BaseService, ICourseService
    {
        public CourseService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<IActionResult> CreateCourse(CourseRequest courseRequest)
        {
            var tutor = await _context.Tutors.FirstOrDefaultAsync(c => c.TutorId == courseRequest.TutorId);
            if (tutor == null)
            {
                return new StatusCodeResult(404);
            }
            if (courseRequest.TotalMoney < 0 || courseRequest.TotalSlots < 0)
            {
                return new StatusCodeResult(400);
            }
            var course = new Course
            {
                CourseId = Guid.NewGuid(),
                TutorId = courseRequest.TutorId,
                CreatedAt = DateTime.UtcNow,
                Description = courseRequest.Description,
                TotalMoney = courseRequest.TotalMoney,
                TotalSlots = courseRequest.TotalSlots,
                Note = courseRequest.Note,
                Status = courseRequest.Status
            };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> UpdateCourse(UpdateCourseRequest courseRequest)
        {
            var tutor = await _context.Tutors.FirstOrDefaultAsync(c => c.TutorId == courseRequest.TutorId);
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseRequest.CourseId);
            if (tutor == null || course == null)
            {
                return new StatusCodeResult(404);
            }
            if (courseRequest.TotalMoney < 0 || courseRequest.TotalSlots < 0)
            {
                return new StatusCodeResult(400);
            }
            if (courseRequest.Description != null)
            {
                course.Description = courseRequest.Description;
            }
            if (courseRequest.TotalMoney > 0 && courseRequest.TotalMoney != course.TotalMoney)
            {
                course.TotalMoney = courseRequest.TotalMoney;
            }
            if (courseRequest.TotalSlots > 0 && courseRequest.TotalSlots != course.TotalSlots)
            {
                course.TotalSlots = courseRequest.TotalSlots;
            }
            if (courseRequest.Note != null && !courseRequest.Note.Equals(course.Note))
            {
                course.Note = courseRequest.Note;
            }
            if (courseRequest.Status > 0 && courseRequest.Status != course.Status)
            {
                course.Status = courseRequest.Status;
            }
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            try
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == id);
                if (course == null)
                {
                    return new StatusCodeResult(404);
                }
                var courseTransactions = await _context.CourseTransactions.AnyAsync(c => c.CourseId == id);
                if (courseTransactions == true)
                {
                    if (course.Status != (Int32)CourseEnum.Deleted)
                    {
                        course.Status = (Int32)CourseEnum.Inactive;
                        _context.Courses.Update(course);
                        var courseOutlines = await _context.CourseOutlines.Where(c => c.CourseId == id).ToListAsync();
                        foreach (var courseOutline in courseOutlines)
                        {
                            courseOutline.Status = (Int32)CourseEnum.Inactive;
                            _context.CourseOutlines.Update(courseOutline);
                        }
                        await _context.SaveChangesAsync();
                        return new StatusCodeResult(200);
                    }
                    else
                    {
                        _context.Courses.Remove(course);
                        var coursePromotions = await _context.CoursePromotions.Where(c => c.CourseId == id).ToListAsync();
                        foreach (var coursePromotion in coursePromotions)
                        {
                            _context.CoursePromotions.Remove(coursePromotion);
                        }
                        var courseOutlines = await _context.CourseOutlines.Where(c => c.CourseId == id).ToListAsync();
                        foreach (var courseOutline in courseOutlines)
                        {
                            _context.CourseOutlines.Remove(courseOutline);
                        }
                        var studentCourses = await _context.StudentCourses.Where(c => c.CourseId == id).ToListAsync();
                        foreach (var studentCourse in studentCourses)
                        {
                            var schedules = await _context.Schedules.Where(c => c.StudentCourseId == studentCourse.StudentCourseId).ToListAsync();
                            _context.Schedules.RemoveRange(schedules);
                            _context.StudentCourses.Remove(studentCourse);
                        }
                        await _context.SaveChangesAsync();
                        return new StatusCodeResult(204);
                    }
                }
                else return new StatusCodeResult(409);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return new StatusCodeResult(500);
        }
        public async Task<IActionResult> CreateCourseOutline(CourseOutlineRequest courseOutlineRequest)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseOutlineRequest.CourseId && c.Status == (Int32)CourseEnum.Active);
            if (course == null)
            {
                return new StatusCodeResult(404);
            }
            var courseOutline = new CourseOutline
            {
                CourseOutlineId = Guid.NewGuid(),
                CourseId = courseOutlineRequest.CourseId,
                Description = courseOutlineRequest.Description,
                Title = courseOutlineRequest.Title,
                Status = courseOutlineRequest.Status
            };
            _context.CourseOutlines.Add(courseOutline);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> UpdateCourseOutline(UpdateCourseOutlineRequest courseOutlineRequest)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseOutlineRequest.CourseId && c.Status == (Int32)CourseEnum.Active);
            var courseOutline = await _context.CourseOutlines.FirstOrDefaultAsync(c => c.CourseOutlineId == courseOutlineRequest.CourseOutlineId && c.Status != (Int32)CourseEnum.Deleted);
            if (course == null || courseOutline == null)
            {
                return new StatusCodeResult(404);
            }
            if (courseOutlineRequest.Description != null)
            {
                courseOutline.Description = courseOutlineRequest.Description;
            }
            if (courseOutlineRequest.Title != null)
            {
                courseOutline.Title = courseOutlineRequest.Title;
            }
            if (courseOutlineRequest.Status > 0 && courseOutlineRequest.Status != courseOutline.Status)
            {
                courseOutline.Status = courseOutlineRequest.Status;
            }
            _context.CourseOutlines.Update(courseOutline);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> DeleteCourseOutline(Guid id)
        {
            var courseOutline = await _context.CourseOutlines.FirstOrDefaultAsync(c => c.CourseOutlineId == id);
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseOutline.CourseId && c.Status != (Int32)CourseEnum.Active);
            if(course != null)
            {
                return new StatusCodeResult(409);
            }
            if (courseOutline == null)
            {
                return new StatusCodeResult(404);
            }
            if(course.Status == (Int32)CourseEnum.Deleted)
            {
                courseOutline.Status = (Int32)CourseEnum.Deleted;
                _context.CourseOutlines.Update(courseOutline);
                await _context.SaveChangesAsync();
                return new StatusCodeResult(200);
            }
            else
            {
                courseOutline.Status = (Int32)CourseEnum.Inactive;
                _context.CourseOutlines.Update(courseOutline);
                await _context.SaveChangesAsync();
                return new StatusCodeResult(200);
            }
            
        }
        public async Task<IActionResult> CreateCoursePromotion(CoursePromotionRequest coursePromotionRequest)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == coursePromotionRequest.CourseId && c.Status == (Int32)CourseEnum.Active);
            var promotion = await _context.Promotions.FirstOrDefaultAsync(c => c.PromotionId == coursePromotionRequest.PromotionId);
            if (course == null || promotion == null)
            {
                return new StatusCodeResult(404);
            }
            var coursePromotion = new CoursePromotion
            {
                CourseId = coursePromotionRequest.CourseId,
                PromotionId = coursePromotionRequest.PromotionId
            };
            _context.CoursePromotions.Add(coursePromotion);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> DeleteCoursePromotion(CoursePromotionRequest coursePromotionRequest)
        {
            var coursePromotion = await _context.CoursePromotions.FirstOrDefaultAsync(c => c.CourseId == coursePromotionRequest.CourseId && c.PromotionId == coursePromotionRequest.PromotionId);
            if (coursePromotion == null)
            {
                return new StatusCodeResult(404);
            }
            _context.CoursePromotions.Remove(coursePromotion);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(204);
        }
        public async Task<IActionResult> UpdateCoursePromotion(CoursePromotionRequest coursePromotionRequest)
        {
            var coursePromotion = await _context.CoursePromotions.FirstOrDefaultAsync(c => c.CourseId == coursePromotionRequest.CourseId && c.PromotionId == coursePromotionRequest.PromotionId);
            if (coursePromotion == null)
            {
                return new StatusCodeResult(404);
            }
            coursePromotion.PromotionId = coursePromotionRequest.PromotionId;
            _context.CoursePromotions.Update(coursePromotion);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> CreatePromotion(CreatePromotion createPromotion)
        {
            var promotionExist = await _context.Promotions.AnyAsync(c => c.PromotionCode == createPromotion.PromotionCode); //moi ma 1 loai phan tram thoi. Ex: VARLUOTDIOLE, PEREZMUATAI
            if (promotionExist)
            {
                return new StatusCodeResult(409);
            }
            var promotion = new Promotion
            {
                PromotionId = Guid.NewGuid(),
                PromotionCode = createPromotion.PromotionCode.ToUpper(),
                Percentage = createPromotion.Percentage,
                CreatedAt = DateTime.UtcNow
            };
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> UpdatePromotion(UpdatePromotion updatePromotion)
        {
            var promotion = await _context.Promotions.FirstOrDefaultAsync(c => c.PromotionId == updatePromotion.PromotionId);
            if (promotion == null)
            {
                return new StatusCodeResult(404);
            }
            promotion.PromotionCode = updatePromotion.PromotionCode.ToUpper();
            promotion.Percentage = updatePromotion.Percentage;
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> DeletePromotion(Guid id)
        {
            var promotion = await _context.Promotions.FirstOrDefaultAsync(c => c.PromotionId == id);
            if (promotion == null)
            {
                return new StatusCodeResult(404);
            }
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(204);
        }
        public async Task<ActionResult<List<Course>>> GetAllCourses()
        {
            try
            {
                var courses = await _context.Courses.ToListAsync();
                if (courses == null)
                {
                    return new StatusCodeResult(404);
                }
                return courses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Course>> GetCourse(Guid id)
        {
            try
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == id);
                if (course == null)
                {
                    return new StatusCodeResult(404);
                }
                return course;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<CourseOutline>>> GetAllCourseOutlines()
        {
            try
            {
                var courseOutlines = await _context.CourseOutlines.ToListAsync();
                if (courseOutlines == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseOutlines;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<CourseOutline>> GetCourseOutline(Guid id)
        {
            try
            {
                var courseOutline = await _context.CourseOutlines.FirstOrDefaultAsync(c => c.CourseOutlineId == id);
                if (courseOutline == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseOutline;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<CoursePromotion>>> GetAllCoursePromotions()
        {
            try
            {
                var coursePromotions = await _context.CoursePromotions.ToListAsync();
                if (coursePromotions == null)
                {
                    return new StatusCodeResult(404);
                }
                return coursePromotions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<CoursePromotion>> GetCoursePromotion(Guid id)
        {
            try
            {
                var coursePromotion = await _context.CoursePromotions.FirstOrDefaultAsync(c => c.CourseId == id || c.PromotionId == id);
                if (coursePromotion == null)
                {
                    return new StatusCodeResult(404);
                }
                return coursePromotion;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Promotion>>> GetAllPromotions()
        {
            try
            {
                var promotions = await _context.Promotions.ToListAsync();
                if (promotions == null)
                {
                    return new StatusCodeResult(404);
                }
                return promotions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Promotion>> GetPromotion(Guid id)
        {
            try
            {
                var promotion = await _context.Promotions.FirstOrDefaultAsync(c => c.PromotionId == id);
                if (promotion == null)
                {
                    return new StatusCodeResult(404);
                }
                return promotion;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
