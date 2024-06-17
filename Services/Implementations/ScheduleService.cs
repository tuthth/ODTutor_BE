using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
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
