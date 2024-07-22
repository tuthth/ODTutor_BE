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
    public interface IScheduleService
    {
        Task<IActionResult> CreateSchedulesForStudentCourse(ScheduleRequest scheduleRequest);
        Task<IActionResult> ReScheduleForStudentCourse(ScheduleRequest scheduleRequest);
        Task<IActionResult> RescheduleSlot(RescheduleRequest rescheduleRequest);
        Task<IActionResult> StartSchedule(Guid scheduleId);
        Task<IActionResult> FinishSchedule(Guid scheduleId);
        Task<ActionResult<List<Schedule>>> GetAllSchedules();
        Task<ActionResult<Schedule>> GetSchedule(Guid id);
        Task<ActionResult<List<Schedule>>> GetSchedulesByStudentCourseId(Guid id);
    }
}
