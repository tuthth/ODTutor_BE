using AutoMapper;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Emails;
using Models.Models.Requests;
using Models.Models.Views;
using Models.PageHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class TutorRegisterService : BaseService, ITutorRegisterService
    {

        private readonly IConfiguration _cf;
        private readonly IWebHostEnvironment _env;
        private readonly IFirebaseRealtimeDatabaseService _firebaseService;
        public TutorRegisterService(ODTutorContext odContext, IWebHostEnvironment env, IMapper mapper, IConfiguration cf, IFirebaseRealtimeDatabaseService firebaseService) : base(odContext, mapper)
        {
            _cf = cf;
            _env = env;
            _firebaseService = firebaseService;
        }

        /*Register Tutor Step By Step*/
        // Register Tutor Information
        // Step 1 : Get Information
        public async Task<ActionResult<TutorRegisterStepOneResponse>> RegisterTutorInformation(TutorInformationRequest tutorRequest, List<Guid> tutorSubjectId)
        {
            try
            {
                var user = findUserByUserID(tutorRequest.UserId);
                if (user == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "User not found", "");
                }
                //Map and save tutor information
                Tutor tutor = _mapper.Map<Tutor>(tutorRequest);
                tutor.TutorId = Guid.NewGuid();
                tutor.Status = (Int32)TutorEnum.Inprocessing; // "2" is InProcessing
                tutor.CreateAt = DateTime.UtcNow.AddHours(7);
                tutor.UpdateAt = DateTime.UtcNow.AddHours(7);
                _context.Tutors.Add(tutor);
                await _context.SaveChangesAsync();
                // Add Tutor Subject List
                List<TutorSubject> tutorSubject = new List<TutorSubject>();
                foreach (var subjectID in tutorSubjectId)
                {
                    TutorSubject tutorSubject1 = new TutorSubject();
                    tutorSubject1.TutorSubjectId = Guid.NewGuid();
                    tutorSubject1.TutorId = tutor.TutorId;
                    tutorSubject1.SubjectId = subjectID;
                    tutorSubject1.CreatedAt = DateTime.UtcNow.AddHours(7);
                    tutorSubject.Add(tutorSubject1);
                }
                if (tutorSubject.Count < 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "You have to choose at least 1 subject", "");
                }
                else
                {
                    _context.TutorSubjects.AddRange(tutorSubject);
                    await _context.SaveChangesAsync();
                }
                var response = new TutorRegisterStepOneResponse()
                {
                    TutorID = tutor.TutorId
                };
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Register Tutor Certificate
        // Step 2 : Get Certificate
        public async Task<IActionResult> TutorCertificatesRegister(Guid tutorID, List<TutorRegisterCertificateRequest> tutorCertificateRequest)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
            if (tutor == null)
            {
                return new StatusCodeResult(404);
            }
            try
            {
                // List all of certificate images
                foreach (var urlLink in tutorCertificateRequest)
                {
                    TutorCertificate certificate = new TutorCertificate();
                    certificate.TutorId = tutorID;
                    certificate.ImageUrl = urlLink.ImageUrl;
                    certificate.CertificateFrom = urlLink.CertificateFrom;
                    certificate.CertificateName = urlLink.CertificateName;
                    certificate.CertificateDescription = urlLink.CertificateDescription;
                    certificate.StartYear = urlLink.StartYear;
                    certificate.EndYear = urlLink.EndYear;
                    certificate.IsVerified = false;
                    _context.TutorCertificates.Add(certificate);
                    await _context.SaveChangesAsync();
                }
                throw new CrudException(HttpStatusCode.Created, "Certificate is created", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Register Tutor Experience
        // Step 3 : Get Experience
        public async Task<IActionResult> RegisterTutorExperience(Guid tutorID, List<TutorExperienceRequest> tutorExperienceRegistList)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
            if (tutor == null)
            {
                throw new CrudException(HttpStatusCode.OK, "Not Found Tutor", "");
            }
            try
            {
                foreach (var tutorExperienceRegist in tutorExperienceRegistList)
                {
                    TutorExperience tutorExperience = _mapper.Map<TutorExperience>(tutorExperienceRegist);
                    tutorExperience.TutorExperienceId = Guid.NewGuid();
                    tutorExperience.TutorId = tutorID;
                    tutorExperience.IsVerified = false;
                    _context.TutorExperiences.Add(tutorExperience);
                }
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.Created, "Experience evidences are saved!", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        //Register Tutor Sub Information
        // Step 4: Get Sub Information
        public async Task<IActionResult> RegisterSubTutorInformation( Guid tutorId, TutorSubInformationRequest tutorSubInformationRequest)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == tutorId).FirstOrDefaultAsync();
            if (tutor == null)
            {
                throw new CrudException(HttpStatusCode.OK, "Not Found Tutor", "");
            }
            try
            {
                _mapper.Map(tutorSubInformationRequest, tutor); //error here, use for update
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.Created, "Tutor Sub Information is saved", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Check, Confirm and Send Notification
        // Step 6: Check, Confirm and Send Notification
        public async Task<IActionResult> CheckConfirmTutorInformationAndSendNotification(TutorConfirmRequest request)
        {
            try
            {
                var tutor = await _context.Tutors.Where(x => x.TutorId == request.TutorID).FirstOrDefaultAsync();
                //Update Tutor Money
                tutor.PricePerHour = request.Price;
                tutor.Status = (Int32)TutorEnum.Pending;
                await _context.SaveChangesAsync();
                // Create a Tutor Action Log
                TutorAction tutorRegister = new TutorAction();
                tutorRegister.TutorActionId = Guid.NewGuid();
                tutorRegister.TutorId = request.TutorID;
                tutorRegister.CreateAt = DateTime.UtcNow.AddHours(7);
                tutorRegister.Description = "Xử lý xét duyệt gia sư";
                tutorRegister.ActionType = (Int32)TutorActionTypeEnum.TutorRegister;
                tutorRegister.Status = (Int32)TutorActionEnum.Pending;
                await _context.TutorActions.AddAsync(tutorRegister);
                await _context.SaveChangesAsync();
                // Create a notification for user who want to become a tutor
                NotificationDTO notification = new NotificationDTO();
                notification.NotificationId = new Guid();
                notification.UserId = tutor.UserId;
                notification.Title = "Yêu cầu xét duyệt thành gia sư đã được gửi";
                notification.Content = "Yêu cầu của bạn đã được gửi. Vui lòng đợi phản hồi qua email hoặc thông báo của hệ thống";
                notification.CreatedAt = DateTime.UtcNow.AddHours(7);
                notification.Status = 1; // "1" is sent
                Notification notification1x = _mapper.Map<Notification>(notification);
                await _context.Notifications.AddAsync(notification1x);
                _firebaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                await _context.SaveChangesAsync();
                return new StatusCodeResult(201);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Register Tutor Schedule
        // Step 5: Create Schedule for Tutor
        public async Task<IActionResult> CreateTutorSlotSchedule(TutorRegistScheduleRequest tutorRegistScheduleRequest)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == tutorRegistScheduleRequest.TutorID).FirstOrDefaultAsync();
            if (tutor == null)
            {
                return new StatusCodeResult(404);
            }
            if (tutorRegistScheduleRequest.StartTime >= tutorRegistScheduleRequest.EndTime)
            {
                return new StatusCodeResult(400);
            }
            if (tutorRegistScheduleRequest.StartTime.TimeOfDay > tutorRegistScheduleRequest.EndTime.TimeOfDay)
            {
                return new StatusCodeResult(409);
            }
            try
            {
                // Calculate the number of weeks between StartTime and EndTime
                int weeks = (int)Math.Ceiling((tutorRegistScheduleRequest.EndTime - tutorRegistScheduleRequest.StartTime).TotalDays / 7);
                DateTime startOfWeek = tutorRegistScheduleRequest.StartTime.Date;
                DateTime endOfWeek;

                // Create a new TutorWeekAvailable for each week
                for (int i = 0; i < weeks; i++)
                {
                    endOfWeek = startOfWeek.AddDays(6); // End of the week is 6 days after the start of the week

                    TutorWeekAvailable tutorWeekAvailable = new TutorWeekAvailable();
                    tutorWeekAvailable.TutorWeekAvailableId = Guid.NewGuid();
                    tutorWeekAvailable.TutorId = tutorRegistScheduleRequest.TutorID;
                    tutorWeekAvailable.StartTime = startOfWeek;
                    tutorWeekAvailable.EndTime = endOfWeek < tutorRegistScheduleRequest.EndTime ? endOfWeek : tutorRegistScheduleRequest.EndTime;
                    _context.TutorWeekAvailables.Add(tutorWeekAvailable);

                    // Create a new TutorDateAvailable for each day in the current week
                    for (var date = startOfWeek; date <= tutorWeekAvailable.EndTime; date = date.AddDays(1))
                    {
                        TutorDateAvailable tutorDateAvailable = new TutorDateAvailable();
                        tutorDateAvailable.TutorDateAvailableID = Guid.NewGuid();
                        tutorDateAvailable.TutorID = tutorRegistScheduleRequest.TutorID;
                        tutorDateAvailable.TutorWeekAvailableID = tutorWeekAvailable.TutorWeekAvailableId;
                        tutorDateAvailable.Date = date;
                        tutorDateAvailable.DayOfWeek = (int)date.DayOfWeek;
                        tutorDateAvailable.StartTime = tutorRegistScheduleRequest.StartTime.TimeOfDay;
                        tutorDateAvailable.EndTime = tutorRegistScheduleRequest.EndTime.TimeOfDay;
                        _context.TutorDateAvailables.Add(tutorDateAvailable);
                        await _context.SaveChangesAsync();
                        // Generate Slot Based On Date
                        var slotList = await generateSlotBasedOnProvidedDate(tutorRegistScheduleRequest.TutorID, tutorDateAvailable);
                    }

                    startOfWeek = startOfWeek.AddDays(7); // Move to the next week
                }

                await _context.SaveChangesAsync();
                return new StatusCodeResult(201);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Create Tutor Schedule Part 2
        // Step 5: Create Schedule for Tutor
        public async Task<IActionResult> CreateTutorSlotInRegisterTutorStep(Guid TutorId, List<TutorRegisterSlotRequest> request)
        {
            try
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                // Get Viet Nam Time 
                DateTime currentDateTimeUtc = DateTime.UtcNow;
                DateTime currentDateTimeVietNam = TimeZoneInfo.ConvertTimeFromUtc(currentDateTimeUtc, vietnamTimeZone);

                // Caculate the nearest Monday 
                DateTime nextMonday = GetNextMonday(currentDateTimeVietNam);
                List<DateTime> WeekDayFromMonday = new List<DateTime>();
                for (int i = 0; i < 7; i++)
                {
                    WeekDayFromMonday.Add(nextMonday.AddDays(i));
                }
                // Create Week Available
                TutorWeekAvailable tutorWeekAvailable = new TutorWeekAvailable();
                tutorWeekAvailable.TutorWeekAvailableId = Guid.NewGuid();
                tutorWeekAvailable.TutorId = TutorId;
                tutorWeekAvailable.StartTime = WeekDayFromMonday[0];
                tutorWeekAvailable.EndTime = WeekDayFromMonday[6];
                await _context.TutorWeekAvailables.AddAsync(tutorWeekAvailable);
                await _context.SaveChangesAsync();
                // Create Date Availble
                List<TutorDateAvailable> tutorDateAvailables = new List<TutorDateAvailable>();
                foreach (var date in WeekDayFromMonday)
                {
                    int dayOfWeek = (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
                    var requestDate = request.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);
                    if (requestDate != null)
                    {
                        foreach (var timeSlot in requestDate.TutorStartTimeEndTimRegisterRequests)
                        {
                            TutorDateAvailable tutorDateAvailable = new TutorDateAvailable
                            {
                                TutorDateAvailableID = Guid.NewGuid(),
                                TutorID = TutorId,
                                TutorWeekAvailableID = tutorWeekAvailable.TutorWeekAvailableId,
                                Date = date,
                                DayOfWeek = dayOfWeek,
                                StartTime = timeSlot.StartTime,
                                EndTime = timeSlot.EndTime
                            };
                            await _context.TutorDateAvailables.AddAsync(tutorDateAvailable);
                            await _context.SaveChangesAsync();
                            // Generate Slot Based On Date
                            var slotList = await generateSlotBasedOnProvidedDate(TutorId, tutorDateAvailable);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                throw new CrudException(HttpStatusCode.Created, "Slot is created", "");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }
        // Get Tutor Register Information --- Đây là nơi để dành cho những người xử lý tutor và duyệt request
        /*Đây là phần để lấy thông tin để admin hay moderator có thể hiểu và kiểm tra*/
        public async Task<ActionResult<TutorRegisterReponse>> GetTutorRegisterInformtaion(Guid tutorID)
        {
            TutorRegisterReponse response = new TutorRegisterReponse();
            List<string> subjectList = await getAllSubjectOfTutor(tutorID);
            List<string> imagesUrlList = await getAllImagesUrlOfTutor(tutorID);
            try
            {
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                User user = await _context.Tutors.Where(x => x.TutorId == tutorID).Select(x => x.UserNavigation).FirstOrDefaultAsync();
                if (tutor == null)
                {
                    return null;
                }
                else
                {
                    response.TutorId = tutor.TutorId;
                    response.IdentityNumber = tutor.IdentityNumber;
                    response.Description = tutor.Description?.Replace("\n", "") ?? "";
                    response.PricePerHour = tutor.PricePerHour.Value;
                    response.Email = user.Email;
                    response.Username = user.Username;
                    response.ImageUrl = user.ImageUrl;
                    response.Name = user.Name;
                    response.Subjects = subjectList;
                    response.ImagesCertificateUrl = imagesUrlList;
                    response.Status = tutor.Status;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        // Get All Tutor Information
        public async Task<ActionResult<List<TutorRegisterReponse>>> GetAllTutorRegisterInformation()
        {
            List<TutorRegisterReponse> responses = new List<TutorRegisterReponse>();
            try
            {
                List<Tutor> tutors = await _context.Tutors.Where(t => t.Status == (Int32)TutorEnum.Pending).ToListAsync();
                foreach (var tutor in tutors)
                {
                    TutorRegisterReponse response = new TutorRegisterReponse();
                    List<string> subjectList = await getAllSubjectOfTutor(tutor.TutorId);
                    List<string> imagesUrlList = await getAllImagesUrlOfTutor(tutor.TutorId);

                    User user = await _context.Users.Where(x => x.Id == tutor.UserId).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        response.TutorId = tutor.TutorId;
                        response.IdentityNumber = tutor.IdentityNumber;
                        response.Description = tutor.Description?.Replace("\n", "") ?? "";
                        response.PricePerHour = tutor.PricePerHour.Value;
                        response.Email = user.Email;
                        response.Username = user.Username;
                        response.ImageUrl = user.ImageUrl;
                        response.Name = user.Name;
                        response.Subjects = subjectList;
                        response.ImagesCertificateUrl = imagesUrlList;
                        response.Status = tutor.Status;
                        responses.Add(response);
                    }
                }
                return responses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<ActionResult<TutorAction>> GetTutorActionById(Guid id)
        {
            var tutorAction = await _context.TutorActions.Where(x => x.TutorActionId == id).FirstOrDefaultAsync();
            if (tutorAction == null)
            {
                return null;
            }
            return tutorAction;
        }
        // Get Tutor Actions
        public async Task<ActionResult<PageResults<TutorAction>>> GetTutorActionByTutorId(Guid id, int size, int pageSize)
        {
            var tutor = await _context.Tutors.Where(x => x.TutorId == id).FirstOrDefaultAsync();
            if (tutor == null)
            {
                return null;
            }
            try
            {
                var tutorActions = await _context.TutorActions.Where(x => x.TutorId == id).OrderByDescending(x => x.CreateAt).ToListAsync();
                var pageResults = PagingHelper<TutorAction>.Paging(tutorActions, size, pageSize);
                return pageResults;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Approval Tutor Register 
        public async Task<IActionResult> ApproveTheTutorRegister(TutorApprovalRequest request)
        {
            try
            {
                TutorAction tutorAction = _context.TutorActions.FirstOrDefault(ta => ta.TutorActionId == request.TutorActionId);
                if (tutorAction == null)
                {
                    return new StatusCodeResult(404);
                }
                tutorAction.ModeratorId = request.ApprovalID;
                tutorAction.ReponseDate = DateTime.UtcNow.AddHours(7);
                tutorAction.Status = (Int32)TutorActionEnum.Accept;
                var tutorCertificate = _context.TutorCertificates.Where(x => x.TutorId == tutorAction.TutorId).ToList();
                foreach (var cert in tutorCertificate)
                {
                    cert.IsVerified = true;
                }
                _context.TutorCertificates.UpdateRange(tutorCertificate);
                var tutorExperience = _context.TutorExperiences.Where(x => x.TutorId == tutorAction.TutorId).ToList();
                foreach (var exp in tutorExperience)
                {
                    exp.IsVerified = true;
                }
                _context.TutorExperiences.UpdateRange(tutorExperience);
                await _context.SaveChangesAsync();

                // Change the Status Of Tutor
                Tutor tutor = _context.Tutors.FirstOrDefault(t => t.TutorId == tutorAction.TutorId);
                tutor.Status = (Int32)TutorEnum.Active;
                await _context.SaveChangesAsync();

                // Create a notification for Tutor
                NotificationDTO notification = new NotificationDTO();
                notification.NotificationId = Guid.NewGuid();
                notification.UserId = tutor.UserId;
                notification.Title = "Yêu cầu xét duyệt trở thành gia sư đã được chấp nhận";
                notification.Content = "Yêu cầu của bạn đã được chấp nhận. Bạn đã trở thành gia sư trên hệ thống";
                notification.CreatedAt = DateTime.UtcNow.AddHours(7);
                notification.Status = (Int32)NotificationEnum.UnRead;
                Notification notification1x = _mapper.Map<Notification>(notification);
                await _context.Notifications.AddAsync(notification1x);
                _firebaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                await _context.SaveChangesAsync();
                return new StatusCodeResult(200);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Deny Tutor Register
        public async Task<IActionResult> DenyTheTutorRegister(TutorApprovalRequest request)
        {
            try
            {
                TutorAction tutorAction = _context.TutorActions.FirstOrDefault(ta => ta.TutorActionId == request.TutorActionId);
                if (tutorAction == null)
                {
                    return new StatusCodeResult(404);
                }
                tutorAction.ModeratorId = request.ApprovalID;
                tutorAction.ReponseDate = DateTime.UtcNow.AddHours(7);
                tutorAction.Status = (Int32)TutorActionEnum.Reject;
                await _context.SaveChangesAsync();

                // Change the Status Of Tutor
                Tutor tutor = _context.Tutors.FirstOrDefault(t => t.TutorId == tutorAction.TutorId);
                tutor.Status = (Int32)TutorEnum.Denny;
                var tutorCertificate = _context.TutorCertificates.Where(x => x.TutorId == tutorAction.TutorId).ToList();
                _context.TutorCertificates.RemoveRange(tutorCertificate);
                var tutorExperience = _context.TutorExperiences.Where(x => x.TutorId == tutorAction.TutorId).ToList();
                _context.TutorExperiences.RemoveRange(tutorExperience);
                await _context.SaveChangesAsync();

                // Create a notification for Tutor
                NotificationDTO notification = new NotificationDTO();
                notification.NotificationId = Guid.NewGuid();
                notification.UserId = tutor.UserId;
                notification.Title = "Yêu cầu xét duyệt trở thành gia sư đã bị từ chối";
                notification.Content = "Yêu cầu của bạn đã bị từ chối. Vui lòng liên hệ với hệ thống để biết thêm thông tin.";
                notification.CreatedAt = DateTime.UtcNow.AddHours(7);
                notification.Status = (Int32)NotificationEnum.Deleted;
                Notification notification1x = _mapper.Map<Notification>(notification);
                await _context.Notifications.AddAsync(notification1x);
                _firebaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                await _context.SaveChangesAsync();
                return new StatusCodeResult(200);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Get Tutor Step 1 By Tutor ID 
        public async Task<ActionResult<TutorRegisterStep1Response>> GetTutorStep1ByTutorID(Guid tutorID)
        {
            try
            {   
                TutorRegisterStep1Response response = new TutorRegisterStep1Response();
                User user = await _context.Users.Where(x => x.TutorNavigation.TutorId == tutorID).FirstOrDefaultAsync();
                if(user == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "User not found", "");
                }
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                if (tutor == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor not found", "");
                }
                response.Email = user.Email;
                response.identifyNumber = tutor.IdentityNumber;
                response.imageUrl = user.ImageUrl;
                response.Name = user.Name;
                response.Subjects =  await getAllSubjectOfTutor(tutorID);
                response.videoUrl = tutor.VideoUrl;
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Get Tutor Step 2 By Tutor ID
        public async Task<ActionResult<List<TutorRegisterStep2Response>>> GetTutorStep2ByTutorID(Guid tutorID)
        {
            try
            {   
                List<TutorRegisterStep2Response> response = new List<TutorRegisterStep2Response>();
                User user = await _context.Users.Where(x => x.TutorNavigation.TutorId == tutorID).FirstOrDefaultAsync();
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                if (user == null || tutor == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor not found", "");
                }
                List<TutorCertificate> tutorCertificateList = await _context.TutorCertificates.Where(x => x.TutorId == tutorID).ToListAsync();
                foreach (var cert in tutorCertificateList)
                {
                    response.Add(new TutorRegisterStep2Response
                    {
                        imageUrl = cert.ImageUrl,
                        CertificateDescription = cert.CertificateDescription,
                        CertifiateForm = cert.CertificateFrom,
                        CertificateName = cert.CertificateName,
                        StartYear = cert.StartYear,
                        EndYear = cert.EndYear
                    });
                }
                if(response.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.OK, "Không ghi nhận chứng chỉ từ gia sư", "");
                }
                return response;
            } catch (CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Get Tutor Step 3 By Tutor ID
        public async Task<ActionResult<List<TutorRegisterStep3Response>>> GetTutorStep3ByTutorID (Guid tutorID)
        {
            try
            {
                List<TutorRegisterStep3Response> response = new List<TutorRegisterStep3Response>();
                User user = await _context.Users.Where(x => x.TutorNavigation.TutorId == tutorID).FirstOrDefaultAsync();
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                if (user == null || tutor == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor not found", "");
                }
                List<TutorExperience> tutorExperiencesList = await _context.TutorExperiences.Where(x => x.TutorId == tutorID).ToListAsync();
                foreach (var experience in tutorExperiencesList)
                {
                    response.Add(new TutorRegisterStep3Response
                    {
                        imageUrl = experience.imageUrl,
                        Title = experience.Title,
                        Description = experience.Description,
                        Location = experience.Location,
                        StartDate = experience.StartDate,
                        EndYear = experience.EndYear
                    });
                }
                if (response.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.OK, "Không ghi nhận kinh nghiệm từ gia sư", "");
                }
                return response;
            }
            catch (CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Get Tutor Step 4 By Tutor ID
        public async Task<ActionResult<TutorRegisterStep4Response>> GetTutorStep4ByTutorID (Guid tutorID)
        {
            try
            {
                TutorRegisterStep4Response response = new TutorRegisterStep4Response();
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                if (tutor == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor not found", "");
                }
                response.Description = tutor.Description;
                response.EducationExperience = tutor.EducationExperience;
                response.Motivation = tutor.Motivation;
                response.AttractiveTitle = tutor.AttractiveTitle;
                return response;
            }
            catch (CrudException ex)
            {
                throw ex;
            }catch(Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Get Tutor Step 5 By TutorID
        public async Task <ActionResult<List<TutorRegisterStep5Reponse>>> GetTutorStep5ByTutorID (Guid tutorID)
        {
            try
            {
                List<TutorRegisterStep5Reponse> response = new List<TutorRegisterStep5Reponse>();
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                if (tutor == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor not found", "");
                }
                List<TutorDateAvailable> tutorDateAvailableList = await _context.TutorDateAvailables.Where(x => x.TutorID == tutorID).ToListAsync();
                if (tutorDateAvailableList.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.OK, "Chưa ghi nhận thời gian đăng ký của Tutor", "");
                }
                foreach (var date in tutorDateAvailableList)
                {
                    response.Add(new TutorRegisterStep5Reponse
                    {
                        dayOfWeek = date.DayOfWeek,
                        startTime = date.StartTime,
                        endTime = date.EndTime
                    });
                }
                return response;
            } catch(CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Get Tutor Step 6 By TutorID
        public async Task <ActionResult<TutorRegisterStep6Response>> GetTutorStep6TutorID (Guid tutorID)
        {
            try
            {
                TutorRegisterStep6Response response = new TutorRegisterStep6Response();
                Tutor tutor = await _context.Tutors.Where(x => x.TutorId == tutorID).FirstOrDefaultAsync();
                if(tutor == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Tutor not found", "");
                }
                response.price = tutor.PricePerHour;
                if (tutor.PricePerHour == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Chưa ghi nhận giá tiền đăng ký", "");
                }
                return response;
            } catch (CrudException ex)
            {
                throw ex;
            } catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }
        /*-------Internal Site---------*/

        // Get All Tutor Subject List
        private async Task<List<string>> getAllSubjectOfTutor(Guid TutorId)
        {
            List<string> subjectlist = new List<string>();
            try
            {
                subjectlist = _context.TutorSubjects.Where(x => x.TutorId == TutorId).Select(x => x.SubjectNavigation.Title).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return subjectlist;
        }

        // Get All Certificate Image Url
        private async Task<List<string>> getAllImagesUrlOfTutor(Guid TutorId)
        {
            List<string> imagesUrlList = new List<string>();
            try
            {
                imagesUrlList = _context.Users.Where(x => x.TutorNavigation.TutorId == TutorId)
                                .Select(x => x.TutorNavigation.TutorCertificatesNavigation
                                .Select(x => x.ImageUrl).ToList()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return imagesUrlList;
        }

        // Find User By UserID 
        private User findUserByUserID(Guid userID)
        {
            User user = _context.Users
                .FirstOrDefault(x => x.Id == userID);
            return user;
        }

        // Check the Tutor Information 

        // Check the Tutor Subject 
        private async Task<bool> checkTutorSubject(Guid tutorId)
        {
            List<TutorSubject> list = new List<TutorSubject>();
            try
            {
                list = await _context.TutorSubjects.Where(ts => ts.TutorId == tutorId).ToListAsync();
                return list.Any();
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Check the Tutor Certificate 
        private async Task<bool> checkTutorCertificate(Guid tutorId)
        {
            List<TutorCertificate> list = new List<TutorCertificate>();
            try
            {
                list = _context.TutorCertificates.Where(tc => tc.TutorId == tutorId).ToList();
                return list.Any();
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, ex.Message, "");
            }
        }

        // Create Tutor Slot Based On Date 
        private async Task<List<TutorSlotAvailable>> generateSlotBasedOnProvidedDate(Guid tutorID, TutorDateAvailable date)
        {
            List<TutorSlotAvailable> slotList = new List<TutorSlotAvailable>();
            try
            {
                TimeSpan StartTime = date.StartTime;
                TimeSpan EndTime = date.EndTime;
                while (StartTime < EndTime)
                {
                    TutorSlotAvailable tutorSlot = new TutorSlotAvailable();
                    tutorSlot.TutorSlotAvailableID = Guid.NewGuid();
                    tutorSlot.TutorDateAvailableID = date.TutorDateAvailableID;
                    tutorSlot.TutorID = tutorID;
                    tutorSlot.StartTime = StartTime;
                    tutorSlot.Status = (Int32)TutorSlotAvailabilityEnum.Available;
                    tutorSlot.IsBooked = false;

                    slotList.Add(tutorSlot);

                    StartTime = StartTime.Add(new TimeSpan(1, 0, 0));

                    // Check the time in next slot is over the end time 
                    if (StartTime >= EndTime)
                    {
                        break;
                    }
                }
                await _context.TutorSlotAvailables.AddRangeAsync(slotList);
                await _context.SaveChangesAsync();
                return slotList;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Get the nearest Monday in Vietnam 
        private DateTime GetNextMonday(DateTime date)
        {
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)date.DayOfWeek + 7) % 7;
            if (daysUntilMonday == 0)
            {
                daysUntilMonday = 7;
            }
            return date.AddDays(daysUntilMonday);
        }

        // Create Tutor Subject List
        public async Task<IActionResult> CreateTutorSubjectList(TutorSubjectRegisterRequest request)
        {
            try
            {
                List<TutorSubject> tutorSubjects = new List<TutorSubject>();
                foreach (var subjectID in request.SubjectList)
                {
                    TutorSubject tutorSubject = new TutorSubject();
                    tutorSubject.TutorSubjectId = Guid.NewGuid();
                    tutorSubject.TutorId = request.TutorId;
                    tutorSubject.SubjectId = subjectID;
                    tutorSubject.CreatedAt = DateTime.UtcNow.AddHours(7);
                    tutorSubjects.Add(tutorSubject);
                }
                // Kiểm tra tutor có bị trùng môn đăng kí không
                var tutorSubjectList = await _context.TutorSubjects.Where(ts => ts.TutorId == request.TutorId).ToListAsync();
                foreach (var subject in tutorSubjects)
                {
                    foreach (var tutorSubject in tutorSubjectList)
                    {
                        if (subject.SubjectId == tutorSubject.SubjectId)
                        {
                            throw new CrudException(HttpStatusCode.BadRequest, "Bạn đã đăng kí môn học này rồi", "");
                        }
                    }
                }
                if (tutorSubjects.Count < 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "You have to choose at least 1 subject", "");
                }
                else
                {
                    _context.TutorSubjects.AddRange(tutorSubjects);
                    await _context.SaveChangesAsync();
                }
                throw new CrudException(HttpStatusCode.Created,"Bạn đã tạo môn học thành công","");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Get Tutor Subject List and Paging
        public async Task<ActionResult<PageResults<TutorSubjectListResponse>>> GetTutorSubjectList(Guid tutorID , int size , int Pagesize)
        {
            List<TutorSubject> tutorSubjects = new List<TutorSubject>();
            try
            {
                tutorSubjects = await _context.TutorSubjects.Where(ts => ts.TutorId == tutorID).Include(ts => ts.SubjectNavigation).ToListAsync();
                List<TutorSubjectListResponse> response = new List<TutorSubjectListResponse>();
                foreach (var subject in tutorSubjects)
                {
                    response.Add(new TutorSubjectListResponse
                    {   
                        TutorSubjectId = subject.TutorSubjectId,
                        SubjectName = subject.SubjectNavigation.Title,
                        SubjectDescription = subject.SubjectNavigation.Content,
                        CreatedDate = subject.CreatedAt
                    });
                }
                if(response.Count == 0)
                {
                    throw new CrudException(HttpStatusCode.OK, "Không tìm thấy môn học của gia sư", "");
                }
                var pageResults = PagingHelper<TutorSubjectListResponse>.Paging(response, size, Pagesize);
                return pageResults;
            }
            catch(CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Remove Tutor Subject
        public async Task<IActionResult> RemoveTutorSubject(Guid tutorID, Guid subjectID)
        {
            try
            {
                var tutorSubject = await _context.TutorSubjects.Where(ts => ts.TutorId == tutorID && ts.SubjectId == subjectID).FirstOrDefaultAsync();
                if (tutorSubject == null)
                {
                    throw new CrudException(HttpStatusCode.OK, "Không tìm thấy môn học của gia sư", "");
                }
                _context.TutorSubjects.Remove(tutorSubject);
                await _context.SaveChangesAsync();
                throw new CrudException(HttpStatusCode.OK, "Xóa môn học thành công", "");
            }
            catch(CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
