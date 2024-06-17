using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IScheduleService
    {
        Task<ActionResult<List<Schedule>>> GetAllSchedules();
        Task<ActionResult<Schedule>> GetSchedule(Guid id);
        Task<ActionResult<List<Schedule>>> GetSchedulesByStudentCourseId(Guid id);
    }
}
