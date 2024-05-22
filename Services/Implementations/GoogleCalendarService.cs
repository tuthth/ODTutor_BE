using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
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
        public async Task<IActionResult> CreateCalendarEvent(GGCalendarEventSetting setting)
        {
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
                    CancellationToken.None);

                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential
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
                        TimeZone = setting.TimeZone
                    },
                    End = new EventDateTime()
                    {
                        DateTime = setting.End,
                        TimeZone = setting.TimeZone
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

                return new StatusCodeResult(201);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
           
        }
    }
   
}
