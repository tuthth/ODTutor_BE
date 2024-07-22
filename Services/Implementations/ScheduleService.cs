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
    public class ScheduleService : BaseService, IScheduleService
    {
        public ScheduleService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<IActionResult> CreateSchedulesForStudentCourse(ScheduleRequest scheduleRequest)
        {
            var studentCourse = _context.StudentCourses.FirstOrDefault(c => c.StudentCourseId == scheduleRequest.StudentCourseId);
            if (studentCourse == null)
            {
                return new StatusCodeResult(404);
            }
            if(studentCourse.Status != (Int32)CourseEnum.Active)
            {
                return new StatusCodeResult(409);
            }
            var courseSlots = _context.CourseSlots.Where(c => c.CourseId == studentCourse.CourseId).OrderBy(c => c.SlotNumber).ToList();
            var startTime = scheduleRequest.StartAt;
            var scheduleList = new List<Schedule>();
            foreach (var courseSlot in courseSlots)
            {
                var schedule = new Schedule
                {
                    ScheduleId = Guid.NewGuid(),
                    StudentCourseId = scheduleRequest.StudentCourseId,
                    StartAt = startTime,
                    EndAt = startTime.AddHours(1),
                    Status = (Int32)ScheduleEnum.Pending
                };
                scheduleList.Add(schedule);
                _context.CourseSchedules.Add(new CourseSchedule
                {
                    CourseSlotId = courseSlot.CourseSlotId,
                    ScheduleId = schedule.ScheduleId,
                });
                startTime = startTime.AddDays(1);
            }
            _context.Schedules.AddRange(scheduleList);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> ReScheduleForStudentCourse(ScheduleRequest scheduleRequest)
        {
            var studentCourse = await _context.StudentCourses.FirstOrDefaultAsync(c => c.StudentCourseId == scheduleRequest.StudentCourseId);
            if (studentCourse == null)
            {
                return new StatusCodeResult(404);
            }

            if (studentCourse.Status != (Int32)CourseEnum.Active)
            {
                return new StatusCodeResult(409);
            }

            var now = DateTime.UtcNow.AddHours(7);
            var schedules = await _context.Schedules
                .Where(s => s.StudentCourseId == scheduleRequest.StudentCourseId)
                .ToListAsync();

            if (schedules.Any(s => s.StartAt <= now.AddHours(24)))
            {
                return new StatusCodeResult(406);
            }
            _context.Schedules.RemoveRange(schedules);
            var courseSchedules = await _context.CourseSchedules
                .Where(cs => schedules.Select(s => s.ScheduleId).Contains(cs.ScheduleId))
                .ToListAsync();
            _context.CourseSchedules.RemoveRange(courseSchedules);

            var courseSlots = await _context.CourseSlots
                .Where(c => c.CourseId == studentCourse.CourseId)
                .OrderBy(c => c.SlotNumber)
                .ToListAsync();
            var startTime = scheduleRequest.StartAt;
            var scheduleList = new List<Schedule>();

            foreach (var courseSlot in courseSlots)
            {
                var schedule = new Schedule
                {
                    ScheduleId = Guid.NewGuid(),
                    StudentCourseId = scheduleRequest.StudentCourseId,
                    StartAt = startTime,
                    EndAt = startTime.AddHours(1),
                    Status = (Int32)ScheduleEnum.Pending
                };
                scheduleList.Add(schedule);
                _context.CourseSchedules.Add(new CourseSchedule
                {
                    CourseSlotId = courseSlot.CourseSlotId,
                    ScheduleId = schedule.ScheduleId,
                });
                startTime = startTime.AddDays(1);
            }

            _context.Schedules.AddRange(scheduleList);
            await _context.SaveChangesAsync();

            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> RescheduleSlot(RescheduleRequest rescheduleRequest)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(c => c.ScheduleId == rescheduleRequest.ScheduleId);
            var now = DateTime.UtcNow.AddHours(7);
            if (schedule == null)
            {
                return new StatusCodeResult(404);
            }
            if (schedule.Status != (Int32)ScheduleEnum.Pending)
            {
                return new StatusCodeResult(409);
            }
            if(schedule.StartAt <= now.AddHours(24))
            {
                return new StatusCodeResult(406);
            }
            schedule.StartAt = rescheduleRequest.StartAt;
            schedule.EndAt = rescheduleRequest.StartAt.AddHours(1);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> StartSchedule(Guid scheduleId)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(c => c.ScheduleId == scheduleId);
            if (schedule == null)
            {
                return new StatusCodeResult(404);
            }
            if (schedule.Status != (Int32)ScheduleEnum.Pending)
            {
                return new StatusCodeResult(409);
            }
            schedule.Status = (Int32)ScheduleEnum.Learning;
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<IActionResult> FinishSchedule(Guid scheduleId)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(c => c.ScheduleId == scheduleId);
            if (schedule == null)
            {
                return new StatusCodeResult(404);
            }
            if (schedule.Status != (Int32)ScheduleEnum.Learning)
            {
                return new StatusCodeResult(409);
            }
            schedule.Status = (Int32)ScheduleEnum.Finished;
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<ActionResult<List<Schedule>>> GetAllSchedules()
        {
            try
            {
                var schedules = await _context.Schedules.OrderByDescending(c => c.StartAt).ToListAsync();
                if (schedules == null)
                {
                    return new StatusCodeResult(404);
                }
                return schedules;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Schedule>> GetSchedule(Guid id)
        {
            try
            {
                var schedule = await _context.Schedules.FirstOrDefaultAsync(c => c.ScheduleId == id);
                if (schedule == null)
                {
                    return new StatusCodeResult(404);
                }
                return schedule;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Schedule>>> GetSchedulesByStudentCourseId(Guid id)
        {
            try
            {
                var schedules = await _context.Schedules.Where(c => c.StudentCourseId == id).OrderByDescending(c => c.StartAt).ToListAsync();
                if (schedules == null)
                {
                    return new StatusCodeResult(404);
                }
                return schedules;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
