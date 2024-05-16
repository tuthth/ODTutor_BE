using Microsoft.AspNetCore.Mvc;
using Settings.Google.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IGoogleCalendarService
    {
        Task<IActionResult> CreateCalendarEvent(GGCalendarEventSetting setting, List<GGCalendarEventAttendee> attendees);
    }
}
