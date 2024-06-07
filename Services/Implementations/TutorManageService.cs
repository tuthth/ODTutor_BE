using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Models.Requests;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class TutorManageService : BaseService, ITutorManageService
    {
        public TutorManageService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<IActionResult> DeleteTutorSlots(List<Guid> tutorSlotAvailableIDs)
        {
            var tutorSlots = await _context.TutorSlotAvailables.Where(x => tutorSlotAvailableIDs.Contains(x.TutorSlotAvailableID)).ToListAsync();
            if (tutorSlots == null || tutorSlots.Count == 0)
            {
                return new StatusCodeResult(404);
            }
            try
            {
                _context.TutorSlotAvailables.RemoveRange(tutorSlots);
                await _context.SaveChangesAsync();
                return new StatusCodeResult(204);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> DeleteSlotInTimeRangeInDate(TutorDateRemoveSlotRequest request)
        {
            var tutorDate = await _context.TutorDateAvailables.Where(x => x.TutorDateAvailableID == request.TutorDateAvalaibleID && (x.StartTime >= request.StartTime && x.EndTime <= request.EndTime)).FirstOrDefaultAsync();
            if (tutorDate == null)
            {
                return new StatusCodeResult(404);
            }
            var tutorSlots = await _context.TutorSlotAvailables.Where(x => x.TutorDateAvailableID == tutorDate.TutorDateAvailableID).ToListAsync();
            var tutorSlotsInTimeRange = tutorSlots.Where(x => x.StartTime >= request.StartTime && x.StartTime <= request.EndTime).ToList();
            if (tutorSlotsInTimeRange.Count == 0)
            {
                return new StatusCodeResult(404);
            }
            try
            {
                _context.TutorSlotAvailables.RemoveRange(tutorSlotsInTimeRange);
                if (tutorSlotsInTimeRange.Count == tutorSlots.Count)
                {
                    _context.TutorDateAvailables.Remove(tutorDate);
                }
                await _context.SaveChangesAsync();
                return new StatusCodeResult(204);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> DeleteSlotInTimeRange(TutorDateRemoveRangeRequest request)
        {
            var tutorDates = await _context.TutorDateAvailables.Where(x => x.TutorID == request.TutorId && (x.Date >= request.StartTime && x.Date <= request.EndTime)).ToListAsync();
            if (tutorDates == null || tutorDates.Count == 0)
            {
                return new StatusCodeResult(404);
            }
            try
            {
                var tutorDateIDs = tutorDates.Select(x => x.TutorDateAvailableID).ToList();
                var tutorSlots = await _context.TutorSlotAvailables.Where(x => tutorDateIDs.Contains(x.TutorDateAvailableID)).ToListAsync();

                // Remove slots and dates
                _context.TutorSlotAvailables.RemoveRange(tutorSlots);
                _context.TutorDateAvailables.RemoveRange(tutorDates);

                // Determine if the date range includes a full week and remove corresponding TutorWeekAvailables
                var startOfWeek = request.StartTime.Date.AddDays(-(int)request.StartTime.Date.DayOfWeek + (int)DayOfWeek.Monday);
                var endOfWeek = startOfWeek.AddDays(6);
                while (startOfWeek < request.EndTime.Date)
                {
                    var tutorWeeks = await _context.TutorWeekAvailables.Where(x => x.TutorId == request.TutorId && x.StartTime >= startOfWeek && x.EndTime <= endOfWeek).ToListAsync();
                    _context.TutorWeekAvailables.RemoveRange(tutorWeeks);
                    startOfWeek = startOfWeek.AddDays(7);
                    endOfWeek = startOfWeek.AddDays(6);
                }

                await _context.SaveChangesAsync();
                return new StatusCodeResult(204);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}
