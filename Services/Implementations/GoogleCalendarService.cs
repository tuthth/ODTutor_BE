using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Emails;
using Services.Interfaces;
using Settings.Google.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class GoogleCalendarService : BaseService, IGoogleCalendarService
    {
        public GoogleCalendarService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
        }

        //cai nay` con` loi o server
        public async Task<IActionResult> CreateCalendarEvent(GGCalendarEventSetting setting)
        {
            foreach(var attendee in setting.Attendees)
            {
                var checkEmail = _context.Users.FirstOrDefault(u => u.Email.Equals(attendee.Email) && u.Banned == false);
                if (checkEmail == null)
                {
                    return new StatusCodeResult(404);
                }
            }
            if(setting.Attendees.Count == 0)
            {
                return new StatusCodeResult(400);
            }
            if(setting.Start < DateTime.Now)
            {
                return new StatusCodeResult(409);
            }
            if (setting.End < setting.Start)
            {
                return new StatusCodeResult(406);
            }
            if(setting.RedirectUri == null)
            {
                return new StatusCodeResult(403);
            }
            if(setting.Summary == null)
            {
                setting.Summary = "TestCapstone";
            }
            if(setting.Description == null)
            {
                setting.Description = "TestCapstone";
            }
            try
            {
                string[] scopes = { CalendarService.Scope.Calendar };
                string projectDirectory = Directory.GetCurrentDirectory();
                string credPath = Path.Combine(projectDirectory, "Properties", "credentials.json");

                var clientSecrets = await GoogleClientSecrets.FromFileAsync(credPath);

                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets.Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    null,
                    new LocalServerCodeReceiver(setting.RedirectUri));

                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApiKey = "AIzaSyAiQNS-CznTqJg_5AryVa4cGMhRjqWX8II"
                });

                List<EventAttendee> attendees = new List<EventAttendee>();
                foreach (var attendee in setting.Attendees)
                {
                    attendees.Add(new EventAttendee()
                    {
                        Email = attendee.Email
                    });
                }

                Event calendarEvent = new Event()
                {
                    Summary = setting.Summary,
                    Location = "Google Meet",
                    Description = setting.Description,
                    Start = new EventDateTime()
                    {
                        DateTime = setting.Start,
                        TimeZone = "Asia/Ho_Chi_Minh"
                    },
                    End = new EventDateTime()
                    {
                        DateTime = setting.End,
                        TimeZone = "Asia/Ho_Chi_Minh"
                    },
                    ConferenceData = new ConferenceData()
                    {
                        CreateRequest = new CreateConferenceRequest()
                        {
                            RequestId = "1234abcdef",
                            ConferenceSolutionKey = new ConferenceSolutionKey()
                            {
                                Type = "hangoutsMeet"
                            }
                        }
                    },
                    GuestsCanInviteOthers = false,
                    GuestsCanModify = false,
                    GuestsCanSeeOtherGuests = false,

                    Attendees = attendees
                };

                EventsResource.InsertRequest request = service.Events.Insert(calendarEvent, "67d3c524db000823b1aae5943497649be3bd8485682e5f45dff9aaaac995c07e@group.calendar.google.com");
                request.ConferenceDataVersion = 1;
                Event createdEvent = await request.ExecuteAsync();

                Console.WriteLine("Event created: {0}", createdEvent.HtmlLink);
                Console.WriteLine("Meet link created: {0}", createdEvent.HangoutLink);

                foreach (var attendee in setting.Attendees)
                {
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = attendee.Email,
                        Subject = "Thông báo sự kiện mới",
                        Body = "Đây là link tham gia buổi học tại Google Meet. \nLink tham gia: " + createdEvent.HangoutLink
                    });
                }
                return new JsonResult(new { GoogleMeetLink = createdEvent.HangoutLink});
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }

}
