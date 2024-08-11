using AutoMapper;
using Microsoft.AspNetCore.Http;
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
using Services.Interfaces;
using Settings.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Models.Enumerables.TutorSubscriptionEnum;


namespace Services.Implementations
{
    public class AdminService : BaseService, IAdminService
    {
        private readonly Dictionary<string, TutorSubscriptionSetting> _subscriptions;
        private readonly Dictionary<string, StudentSubscriptionSetting> _studentSubscriptions;
        public AdminService(ODTutorContext odContext, IMapper mapper) : base(odContext, mapper)
        {
            _subscriptions = new Dictionary<string, TutorSubscriptionSetting>();
            _subscriptions["mienPhi"] = _tutorSubscriptionConfiguration.GetSection("mienPhi").Get<TutorSubscriptionSetting>();
            _subscriptions["traiNghiem"] = _tutorSubscriptionConfiguration.GetSection("traiNghiem").Get<TutorSubscriptionSetting>();
            _subscriptions["thanhVien"] = _tutorSubscriptionConfiguration.GetSection("thanhVien").Get<TutorSubscriptionSetting>();

            _studentSubscriptions = new Dictionary<string, StudentSubscriptionSetting>();
            _studentSubscriptions["mienPhi"] = _studentSubscriptionConfiguration.GetSection("mienPhi").Get<StudentSubscriptionSetting>();
            _studentSubscriptions["thanhVien"] = _studentSubscriptionConfiguration.GetSection("thanhVien").Get<StudentSubscriptionSetting>();
        }
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (users == null)
                {
                    return new StatusCodeResult(404);
                }
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<User>>> GetAllUsersPaging(PagingRequest request)
        {
            try
            {
                var usersList = await _context.Users.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (usersList == null || !usersList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedUsers = PagingHelper<User>.Paging(usersList, request.Page, request.PageSize);
                if (paginatedUsers == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedUsers;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);
                if (user == null)
                {
                    return new StatusCodeResult(404);
                }
                return user;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Tutor>>> GetAllTutors()
        {
            try
            {
                var tutors = await _context.Tutors.OrderByDescending(c => c.UserNavigation.CreatedAt).ToListAsync();
                if (tutors == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutors;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Tutor>> GetTutor(Guid id)
        {
            try
            {
                var tutor = await _context.Tutors.FirstOrDefaultAsync(c => c.TutorId == id);
                if (tutor == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutor;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorCertificate>>> GetAllTutorCertificates()
        {
            try
            {
                var tutorCertificates = await _context.TutorCertificates.ToListAsync();
                if (tutorCertificates == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorCertificates;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<TutorCertificate>> GetTutorCertificate(Guid id)
        {
            try
            {
                var tutorCertificate = await _context.TutorCertificates.FirstOrDefaultAsync(c => c.TutorCertificateId == id);
                if (tutorCertificate == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorCertificate;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorCertificate>>> GetTutorCertificatesByTutorId(Guid id)
        {
            try
            {
                var tutorCertificates = await _context.TutorCertificates.Where(c => c.TutorId == id).ToListAsync();
                if (tutorCertificates == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorCertificates;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorCertificate>>> GetTutorCertificatesBySubjectId(Guid id)
        {
            try
            {
                var tutorCertificates = await _context.TutorCertificates.Where(c => c.TutorId == id).ToListAsync();
                if (tutorCertificates == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorCertificates;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorSubject>>> GetAllTutorSubjects()
        {
            try
            {
                var tutorSubjects = await _context.TutorSubjects.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (tutorSubjects == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorSubjects;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<TutorSubject>> GetTutorSubject(Guid id)
        {
            try
            {
                var tutorSubject = await _context.TutorSubjects.FirstOrDefaultAsync(c => c.TutorSubjectId == id);
                if (tutorSubject == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorSubject;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorSubject>>> GetTutorSubjectsByTutorId(Guid id)
        {
            try
            {
                var tutorSubjects = await _context.TutorSubjects.Where(c => c.TutorId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (tutorSubjects == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorSubjects;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorSubject>>> GetTutorSubjectsBySubjectId(Guid id)
        {
            try
            {
                var tutorSubjects = await _context.TutorSubjects.Where(c => c.SubjectId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (tutorSubjects == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorSubjects;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorRating>>> GetAllTutorRatings()
        {
            try
            {
                var tutorRatings = await _context.TutorRatings.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (tutorRatings == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorRatings;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<TutorRating>> GetTutorRating(Guid id)
        {
            try
            {
                var tutorRating = await _context.TutorRatings.FirstOrDefaultAsync(c => c.TutorRatingId == id);
                if (tutorRating == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorRating;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorRating>>> GetTutorRatingsByTutorId(Guid id)
        {
            try
            {
                var tutorRatings = await _context.TutorRatings.Where(c => c.TutorId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (tutorRatings == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorRatings;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorRating>>> GetTutorRatingsByStudentId(Guid id)
        {
            try
            {
                var tutorRatings = await _context.TutorRatings.Where(c => c.StudentId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (tutorRatings == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorRatings;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<TutorRatingImage>>> GetAllTutorRatingImages()
        {
            try
            {
                var tutorRatingImages = await _context.TutorRatingImages.ToListAsync();
                if (tutorRatingImages == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorRatingImages;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<TutorRatingImage>> GetTutorRatingImage(Guid id)
        {
            try
            {
                var tutorRatingImage = await _context.TutorRatingImages.FirstOrDefaultAsync(c => c.TutorRatingImageId == id);
                if (tutorRatingImage == null)
                {
                    return new StatusCodeResult(404);
                }
                return tutorRatingImage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Moderator>>> GetModerators()
        {
            try
            {
                var moderators = await _context.Moderators.OrderByDescending(c => c.UserNavigation.CreatedAt).ToListAsync();
                if (moderators == null)
                {
                    return new StatusCodeResult(404);
                }
                return moderators;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Moderator>> GetModeratorById(Guid id)
        {
            try
            {
                var moderator = await _context.Moderators.FirstOrDefaultAsync(c => c.ModeratorId == id || c.UserId == id);
                if (moderator == null)
                {
                    return new StatusCodeResult(404);
                }
                return moderator;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Notification>>> GetNotifications()
        {
            try
            {
                var notifications = await _context.Notifications.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (notifications == null)
                {
                    return new StatusCodeResult(404);
                }
                return notifications;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<Notification>>> GetNotificationsByUserId(Guid id)
        {
            try
            {
                var notifications = await _context.Notifications.Where(c => c.UserId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (notifications == null)
                {
                    return new StatusCodeResult(404);
                }
                return notifications;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> GetStudentStatisticsByDayOfWeek()
        {
            try
            {
                var students = await _context.Students.Include(c => c.UserNavigation).ToListAsync();
                if (students == null)
                {
                    return new StatusCodeResult(404);
                }

                var daysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

                var studentsByDayAndTime = daysOfWeek.Select(day => new
                {
                    DayOfWeek = (int)day,
                    StudentsRegisteredInMorning = students.Count(s => s.UserNavigation.CreatedAt.DayOfWeek == day && s.UserNavigation.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)),
                    StudentsRegisteredInAfternoon = students.Count(s => s.UserNavigation.CreatedAt.DayOfWeek == day && s.UserNavigation.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && s.UserNavigation.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)),
                    StudentsRegisteredInEvening = students.Count(s => s.UserNavigation.CreatedAt.DayOfWeek == day && s.UserNavigation.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16))
                }).OrderBy(d => d.DayOfWeek).ToList();

                return new JsonResult(new
                {
                    StudentsRegistered = students.Count,
                    StudentsActive = students.Count(c => c.UserNavigation.Active),
                    StudentsBanned = students.Count(c => c.UserNavigation.Banned),
                    StudentsSubscriptions = students.Count(c => c.UserNavigation.HasBoughtSubscription == true),
                    StudentsByDayAndTime = studentsByDayAndTime
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> GetStudentStatisticsByMonth()
        {
            try
            {
                var students = await _context.Students.Include(c => c.UserNavigation).ToListAsync();
                if (students == null)
                {
                    return new StatusCodeResult(404);
                }

                var monthsOfYear = Enumerable.Range(1, 12);

                var studentsByMonthAndTime = monthsOfYear.Select(month => new
                {
                    Month = month,
                    StudentsRegisteredInMorning = students.Count(s => s.UserNavigation.CreatedAt.Month == month && s.UserNavigation.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)),
                    StudentsRegisteredInAfternoon = students.Count(s => s.UserNavigation.CreatedAt.Month == month && s.UserNavigation.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && s.UserNavigation.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)),
                    StudentsRegisteredInEvening = students.Count(s => s.UserNavigation.CreatedAt.Month == month && s.UserNavigation.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16))
                }).OrderBy(d => d.Month).ToList();

                return new JsonResult(new
                {
                    Year = DateTime.Now.Year,
                    StudentsRegistered = students.Count,
                    StudentsActive = students.Count(c => c.UserNavigation.Active),
                    StudentsBanned = students.Count(c => c.UserNavigation.Banned),
                    StudentsSubscriptions = students.Count(c => c.UserNavigation.HasBoughtSubscription == true),
                    StudentsByMonthAndTime = studentsByMonthAndTime
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> GetTutorStatisticsByDayOfWeek()
        {
            try
            {
                var tutors = await _context.Tutors.Include(c => c.UserNavigation).ToListAsync();
                if (tutors == null)
                {
                    return new StatusCodeResult(404);
                }

                var daysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

                var tutorsByDayAndTime = daysOfWeek.Select(day => new
                {
                    DayOfWeek = (int)day,
                    TutorsRegisteredInMorning = tutors.Count(t => t.UserNavigation.CreatedAt.DayOfWeek == day && t.UserNavigation.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)),
                    TutorsRegisteredInAfternoon = tutors.Count(t => t.UserNavigation.CreatedAt.DayOfWeek == day && t.UserNavigation.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && t.UserNavigation.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)),
                    TutorsRegisteredInEvening = tutors.Count(t => t.UserNavigation.CreatedAt.DayOfWeek == day && t.UserNavigation.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16))
                }).OrderBy(d => d.DayOfWeek).ToList();

                return new JsonResult(new
                {
                    TutorsRegistered = tutors.Count,
                    TutorsActive = tutors.Count(c => c.UserNavigation.Active),
                    TutorsBanned = tutors.Count(c => c.UserNavigation.Banned),
                    TutorsPremium = tutors.Count(c => c.UserNavigation.IsPremium),
                    TutorsByDayAndTime = tutorsByDayAndTime
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> GetTutorStatisticsByMonth()
        {
            try
            {
                var tutors = await _context.Tutors.Include(c => c.UserNavigation).ToListAsync();
                if (tutors == null)
                {
                    return new StatusCodeResult(404);
                }

                var monthsOfYear = Enumerable.Range(1, 12);

                var tutorsByMonthAndTime = monthsOfYear.Select(month => new
                {
                    Month = month,
                    TutorsRegisteredInMorning = tutors.Count(t => t.UserNavigation.CreatedAt.Month == month && t.UserNavigation.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)),
                    TutorsRegisteredInAfternoon = tutors.Count(t => t.UserNavigation.CreatedAt.Month == month && t.UserNavigation.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && t.UserNavigation.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)),
                    TutorsRegisteredInEvening = tutors.Count(t => t.UserNavigation.CreatedAt.Month == month && t.UserNavigation.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16))
                }).OrderBy(d => d.Month).ToList();

                return new JsonResult(new
                {
                    Year = DateTime.Now.Year,
                    TutorsRegistered = tutors.Count,
                    TutorsActive = tutors.Count(c => c.UserNavigation.Active),
                    TutorsBanned = tutors.Count(c => c.UserNavigation.Banned),
                    TutorsPremium = tutors.Count(c => c.UserNavigation.IsPremium),
                    TutorsByMonthAndTime = tutorsByMonthAndTime
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> GetBookingStatisticsByMonth()
        {
            try
            {
                var bookings = await _context.Bookings.Include(b => b.StudentNavigation).Include(b => b.TutorNavigation).ToListAsync();
                if (bookings == null)
                {
                    return new StatusCodeResult(404);
                }

                var monthsOfYear = Enumerable.Range(1, 12);

                var bookingsByMonthAndTime = monthsOfYear.Select(month => new
                {
                    Month = month,
                    BookingsInMorning = bookings.Count(b => b.CreatedAt.Month == month && b.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)),
                    BookingsInAfternoon = bookings.Count(b => b.CreatedAt.Month == month && b.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && b.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)),
                    BookingsInEvening = bookings.Count(b => b.CreatedAt.Month == month && b.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)),
                    Finished = bookings.Count(b => b.CreatedAt.Month == month && b.Status == (Int32)BookingEnum.Finished),
                    Cancelled = bookings.Count(b => b.CreatedAt.Month == month && b.Status == (Int32)BookingEnum.Cancelled),
                    Success = bookings.Count(b => b.CreatedAt.Month == month && b.Status == (Int32)BookingEnum.Success),
                    WaitForPayment = bookings.Count(b => b.CreatedAt.Month == month && b.Status == (Int32)BookingEnum.WaitingPayment),
                    Learning = bookings.Count(b => b.CreatedAt.Month == month && b.Status == (Int32)BookingEnum.Learning)
                }).OrderBy(d => d.Month).ToList();

                return new JsonResult(new
                {
                    TotalBookings = bookings.Count,
                    BookingsByMonthAndTime = bookingsByMonthAndTime
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> GetBookingTransactionStatisticsByMonth()
        {
            try
            {
                var bookingTransactions = await _context.BookingTransactions.Include(bt => bt.BookingNavigation).ToListAsync();
                if (bookingTransactions == null)
                {
                    return new StatusCodeResult(404);
                }

                var monthsOfYear = Enumerable.Range(1, 12);

                var transactionsByMonthAndTime = monthsOfYear.Select(month => new
                {
                    Month = month,
                    TransactionsInMorning = bookingTransactions.Count(bt => bt.CreatedAt.Month == month && bt.Status == 0 && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)),
                    TransactionsInAfternoon = bookingTransactions.Count(bt => bt.CreatedAt.Month == month && bt.Status == 0 && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)),
                    TransactionsInEvening = bookingTransactions.Count(bt => bt.CreatedAt.Month == month && bt.Status == 0 && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)),
                    Approved = bookingTransactions.Count(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.APPROVE),
                    Pending = bookingTransactions.Count(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.PENDING),
                    Rejected = bookingTransactions.Count(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.REJECT),
                    TotalRevenues = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.APPROVE).Sum(bt => bt.Amount),
                    TotalRevenuesInMorning = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.APPROVE && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)).Sum(bt => bt.Amount),
                    TotalRevenuesInAfternoon = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.APPROVE && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    TotalRevenuesInEvening = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.APPROVE && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    PendingRevenues = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.PENDING).Sum(bt => bt.Amount),
                    PendingRevenuesInMorning = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.PENDING && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)).Sum(bt => bt.Amount),
                    PendingRevenuesInAfternoon = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.PENDING && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    PendingRevenuesInEvening = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.PENDING && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    RejectedRevenues = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.REJECT).Sum(bt => bt.Amount),
                    RejectedRevenuesInMorning = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.REJECT && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)).Sum(bt => bt.Amount),
                    RejectedRevenuesInAfternoon = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.REJECT && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    RejectedRevenuesInEvening = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.REJECT && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)).Sum(bt => bt.Amount)
                }).OrderBy(d => d.Month).ToList();

                return new JsonResult(new
                {
                    Year = DateTime.Now.Year,
                    TotalTransactions = bookingTransactions.Count,
                    ApprovedTransactions = bookingTransactions.Count(bt => bt.Status == (Int32)VNPayType.APPROVE),
                    PendingTransactions = bookingTransactions.Count(bt => bt.Status == (Int32)VNPayType.PENDING),
                    RejectedTransactions = bookingTransactions.Count(bt => bt.Status == (Int32)VNPayType.REJECT),
                    TotalRevenues = bookingTransactions.Where(bt => bt.Status == (Int32)VNPayType.APPROVE).Sum(bt => bt.Amount),
                    PendingRevenues = bookingTransactions.Where(bt => bt.Status == (Int32)VNPayType.PENDING).Sum(bt => bt.Amount),
                    RejectedRevenues = bookingTransactions.Where(bt => bt.Status == (Int32)VNPayType.REJECT).Sum(bt => bt.Amount),
                    TransactionsByMonthAndTime = transactionsByMonthAndTime
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> GetBookingStatisticsOf1TutorByMonth(Guid tutorId)
        {
            try
            {
                var bookings = await _context.Bookings.Include(b => b.StudentNavigation).Include(b => b.TutorNavigation).Where(b => b.TutorId == tutorId).ToListAsync();
                if (bookings == null)
                {
                    return new StatusCodeResult(404);
                }

                var monthsOfYear = Enumerable.Range(1, 12);

                var bookingsByMonthAndTime = monthsOfYear.Select(month => new
                {
                    Month = month,
                    BookingsInMorning = bookings.Count(b => b.CreatedAt.Month == month && b.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)),
                    BookingsInAfternoon = bookings.Count(b => b.CreatedAt.Month == month && b.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && b.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)),
                    BookingsInEvening = bookings.Count(b => b.CreatedAt.Month == month && b.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)),
                    Finished = bookings.Count(b => b.CreatedAt.Month == month && b.Status == (Int32)BookingEnum.Finished),
                    Cancelled = bookings.Count(b => b.CreatedAt.Month == month && b.Status == (Int32)BookingEnum.Cancelled),
                    Success = bookings.Count(b => b.CreatedAt.Month == month && b.Status == (Int32)BookingEnum.Success),
                    WaitForPayment = bookings.Count(b => b.CreatedAt.Month == month && b.Status == (Int32)BookingEnum.WaitingPayment),
                    Learning = bookings.Count(b => b.CreatedAt.Month == month && b.Status == (Int32)BookingEnum.Learning)
                }).OrderBy(d => d.Month).ToList();

                return new JsonResult(new
                {
                    TotalBookings = bookings.Count,
                    BookingsByMonthAndTime = bookingsByMonthAndTime
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> GetBookingStatisticsTop5TutorsByMonth()
        {
            try
            {
                var bookings = await _context.Bookings.Include(b => b.StudentNavigation).Include(b => b.TutorNavigation).ToListAsync();
                if (bookings == null)
                {
                    return new StatusCodeResult(404);
                }

                var top5Tutors = bookings.GroupBy(b => b.TutorId)
                    .Select(g => new
                    {
                        TutorId = g.Key,
                        BookingsCount = g.Count()
                    })
                    .OrderByDescending(t => t.BookingsCount)
                    .Take(5)
                    .ToList();

                var top5TutorIds = top5Tutors.Select(t => t.TutorId).ToList();

                var bookingsByTop5Tutors = bookings.Where(b => top5TutorIds.Contains(b.TutorId)).ToList();

                var monthsOfYear = Enumerable.Range(1, 12);

                var bookingsByMonthAndTime = monthsOfYear.Select(month => new
                {
                    Month = month,
                    BookingsInMorning = bookingsByTop5Tutors.Count(b => b.CreatedAt.Month == month && b.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)),
                    BookingsInAfternoon = bookingsByTop5Tutors.Count(b => b.CreatedAt.Month == month && b.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && b.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)),
                    BookingsInEvening = bookingsByTop5Tutors.Count(b => b.CreatedAt.Month == month && b.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)),
                    Finished = bookingsByTop5Tutors.Count(b => b.CreatedAt.Month == month && b.Status == (int)BookingEnum.Finished),
                    Cancelled = bookingsByTop5Tutors.Count(b => b.CreatedAt.Month == month && b.Status == (int)BookingEnum.Cancelled),
                    Success = bookingsByTop5Tutors.Count(b => b.CreatedAt.Month == month && b.Status == (int)BookingEnum.Success),
                    WaitForPayment = bookingsByTop5Tutors.Count(b => b.CreatedAt.Month == month && b.Status == (int)BookingEnum.WaitingPayment),
                    Learning = bookingsByTop5Tutors.Count(b => b.CreatedAt.Month == month && b.Status == (int)BookingEnum.Learning)
                }).OrderBy(d => d.Month).ToList();

                return new JsonResult(new
                {
                    TotalBookings = bookingsByTop5Tutors.Count,
                    BookingsByMonthAndTime = bookingsByMonthAndTime,
                    Top5Tutors = top5Tutors
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> GetBookingTransactionStatisticsOfATutorByMonth(Guid receiverId)
        {
            try
            {
                var bookingTransactions = await _context.BookingTransactions.Include(bt => bt.BookingNavigation).Where(bt => bt.ReceiverWalletId == receiverId).ToListAsync();
                if (bookingTransactions == null)
                {
                    return new StatusCodeResult(404);
                }

                var monthsOfYear = Enumerable.Range(1, 12);

                var transactionsByMonthAndTime = monthsOfYear.Select(month => new
                {
                    Month = month,
                    TransactionsInMorning = bookingTransactions.Count(bt => bt.CreatedAt.Month == month && bt.Status == 0 && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)),
                    TransactionsInAfternoon = bookingTransactions.Count(bt => bt.CreatedAt.Month == month && bt.Status == 0 && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)),
                    TransactionsInEvening = bookingTransactions.Count(bt => bt.CreatedAt.Month == month && bt.Status == 0 && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)),
                    Approved = bookingTransactions.Count(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.APPROVE),
                    Pending = bookingTransactions.Count(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.PENDING),
                    Rejected = bookingTransactions.Count(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.REJECT),
                    TotalRevenues = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.APPROVE).Sum(bt => bt.Amount),
                    TotalRevenuesInMorning = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.APPROVE && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)).Sum(bt => bt.Amount),
                    TotalRevenuesInAfternoon = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.APPROVE && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    TotalRevenuesInEvening = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.APPROVE && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    PendingRevenues = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.PENDING).Sum(bt => bt.Amount),
                    PendingRevenuesInMorning = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.PENDING && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)).Sum(bt => bt.Amount),
                    PendingRevenuesInAfternoon = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.PENDING && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    PendingRevenuesInEvening = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.PENDING && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    RejectedRevenues = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.REJECT).Sum(bt => bt.Amount),
                    RejectedRevenuesInMorning = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.REJECT && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)).Sum(bt => bt.Amount),
                    RejectedRevenuesInAfternoon = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.REJECT && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    RejectedRevenuesInEvening = bookingTransactions.Where(bt => bt.CreatedAt.Month == month && bt.Status == (Int32)VNPayType.REJECT && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)).Sum(bt => bt.Amount)
                }).OrderBy(d => d.Month).ToList();

                return new JsonResult(new
                {
                    Year = DateTime.Now.Year,
                    TotalTransactions = bookingTransactions.Count,
                    ApprovedTransactions = bookingTransactions.Count(bt => bt.Status == (Int32)VNPayType.APPROVE),
                    PendingTransactions = bookingTransactions.Count(bt => bt.Status == (Int32)VNPayType.PENDING),
                    RejectedTransactions = bookingTransactions.Count(bt => bt.Status == (Int32)VNPayType.REJECT),
                    TotalRevenues = bookingTransactions.Where(bt => bt.Status == (Int32)VNPayType.APPROVE).Sum(bt => bt.Amount),
                    PendingRevenues = bookingTransactions.Where(bt => bt.Status == (Int32)VNPayType.PENDING).Sum(bt => bt.Amount),
                    RejectedRevenues = bookingTransactions.Where(bt => bt.Status == (Int32)VNPayType.REJECT).Sum(bt => bt.Amount),
                    TransactionsByMonthAndTime = transactionsByMonthAndTime
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IActionResult> GetBookingTransactionStatisticsTop5TutorsByMonth()
        {
            try
            {
                var bookingTransactions = await _context.BookingTransactions.Include(bt => bt.BookingNavigation).ToListAsync();
                if (bookingTransactions == null)
                {
                    return new StatusCodeResult(404);
                }

                var top5Receivers = bookingTransactions.GroupBy(bt => bt.ReceiverWalletId)
                    .Select(g => new
                    {
                        ReceiverWalletId = g.Key,
                        TransactionsCount = g.Count()
                    })
                    .OrderByDescending(r => r.TransactionsCount)
                    .Take(5)
                    .ToList();

                var top5ReceiverIds = top5Receivers.Select(r => r.ReceiverWalletId).ToList();

                var transactionsByTop5Receivers = bookingTransactions.Where(bt => top5ReceiverIds.Contains(bt.ReceiverWalletId)).ToList();

                var monthsOfYear = Enumerable.Range(1, 12);

                var transactionsByMonthAndTime = monthsOfYear.Select(month => new
                {
                    Month = month,
                    TransactionsInMorning = transactionsByTop5Receivers.Count(bt => bt.CreatedAt.Month == month && bt.Status == 0 && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)),
                    TransactionsInAfternoon = transactionsByTop5Receivers.Count(bt => bt.CreatedAt.Month == month && bt.Status == 0 && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)),
                    TransactionsInEvening = transactionsByTop5Receivers.Count(bt => bt.CreatedAt.Month == month && bt.Status == 0 && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)),
                    Approved = transactionsByTop5Receivers.Count(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.APPROVE),
                    Pending = transactionsByTop5Receivers.Count(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.PENDING),
                    Rejected = transactionsByTop5Receivers.Count(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.REJECT),
                    TotalRevenues = transactionsByTop5Receivers.Where(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.APPROVE).Sum(bt => bt.Amount),
                    TotalRevenuesInMorning = transactionsByTop5Receivers.Where(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.APPROVE && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)).Sum(bt => bt.Amount),
                    TotalRevenuesInAfternoon = transactionsByTop5Receivers.Where(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.APPROVE && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    TotalRevenuesInEvening = transactionsByTop5Receivers.Where(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.APPROVE && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    PendingRevenues = transactionsByTop5Receivers.Where(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.PENDING).Sum(bt => bt.Amount),
                    PendingRevenuesInMorning = transactionsByTop5Receivers.Where(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.PENDING && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)).Sum(bt => bt.Amount),
                    PendingRevenuesInAfternoon = transactionsByTop5Receivers.Where(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.PENDING && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    PendingRevenuesInEvening = transactionsByTop5Receivers.Where(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.PENDING && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    RejectedRevenues = transactionsByTop5Receivers.Where(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.REJECT).Sum(bt => bt.Amount),
                    RejectedRevenuesInMorning = transactionsByTop5Receivers.Where(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.REJECT && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(8)).Sum(bt => bt.Amount),
                    RejectedRevenuesInAfternoon = transactionsByTop5Receivers.Where(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.REJECT && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(8) && bt.CreatedAt.TimeOfDay < TimeSpan.FromHours(16)).Sum(bt => bt.Amount),
                    RejectedRevenuesInEvening = transactionsByTop5Receivers.Where(bt => bt.CreatedAt.Month == month && bt.Status == (int)VNPayType.REJECT && bt.CreatedAt.TimeOfDay >= TimeSpan.FromHours(16)).Sum(bt => bt.Amount)
                }).OrderBy(d => d.Month).ToList();

                return new JsonResult(new
                {
                    Year = DateTime.Now.Year,
                    TotalTransactions = transactionsByTop5Receivers.Count,
                    ApprovedTransactions = transactionsByTop5Receivers.Count(bt => bt.Status == (int)VNPayType.APPROVE),
                    PendingTransactions = transactionsByTop5Receivers.Count(bt => bt.Status == (int)VNPayType.PENDING),
                    RejectedTransactions = transactionsByTop5Receivers.Count(bt => bt.Status == (int)VNPayType.REJECT),
                    TotalRevenues = transactionsByTop5Receivers.Where(bt => bt.Status == (int)VNPayType.APPROVE).Sum(bt => bt.Amount),
                    PendingRevenues = transactionsByTop5Receivers.Where(bt => bt.Status == (int)VNPayType.PENDING).Sum(bt => bt.Amount),
                    RejectedRevenues = transactionsByTop5Receivers.Where(bt => bt.Status == (int)VNPayType.REJECT).Sum(bt => bt.Amount),
                    TransactionsByMonthAndTime = transactionsByMonthAndTime,
                    Top5Receivers = top5Receivers
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Accept Tutor Certificate by Moderator or Admin 
        public async Task AcceptTutorCertificate(List<Guid> tutorCertificateId)
        {
            try
            {
                var tutorCertificates = await _context.TutorCertificates.Where(c => tutorCertificateId.Contains(c.TutorCertificateId)).ToListAsync();
                if (tutorCertificates == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not Found", "Tutor Certificate not found");
                }
                foreach (var tutorCertificate in tutorCertificates)
                {
                    tutorCertificate.IsVerified = true;
                    // Gửi mail cho tutor khi được accept 
                    var tutor = await _context.Tutors.FirstOrDefaultAsync(c => c.TutorId == tutorCertificate.TutorId);
                    var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == tutor.UserId);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = user.Email,
                        Subject = "Chứng chỉ đã được xác minh",
                        Body = "Chứng chỉ của bạn đã được hệ thống xác minh và công nhận. Kiểm tra lại nhé nếu có gì sai sót liên hệ với chúng tôi nhé. Chúc bạn có một ngày tốt lành!"
                    });

                    _context.TutorCertificates.Update(tutorCertificate);
                    throw new CrudException(HttpStatusCode.OK, "OK", "Tutor Certificate is accepted");
                }
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Internal Server Error", "");
            }
        }

        // Accept Tutor Experience by Moderator or Admin
        public async Task AcceptTutorExperience(List<Guid> tutorExperienceId)
        {
            try
            {
                var tutorExperiences = await _context.TutorExperiences.Where(c => tutorExperienceId.Contains(c.TutorExperienceId)).ToListAsync();
                if (tutorExperiences == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not Found", "Tutor Experience not found");
                }
                foreach (var tutorExperience in tutorExperiences)
                {
                    tutorExperience.IsVerified = true;
                    // Gửi mail cho tutor khi được accept 
                    var tutor = await _context.Tutors.FirstOrDefaultAsync(c => c.TutorId == tutorExperience.TutorId);
                    var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == tutor.UserId);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = user.Email,
                        Subject = "Kinh nghiệm đã được xác minh",
                        Body = "Kinh nghiệm của bạn đã được hệ thống xác minh và công nhận. Kiểm tra lại nhé nếu có gì sai sót liên hệ với chúng tôi nhé. Chúc bạn có một ngày tốt lành!"
                    });

                    _context.TutorExperiences.Update(tutorExperience);
                    throw new CrudException(HttpStatusCode.OK, "OK", "Tutor Experience is accepted");
                }
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Internal Server Error", "");
            }
        }

        // Deny Tutor Certificate by Moderator or Admin
        public async Task DenyTutorCertificate(List<Guid> tutorCertificateId)
        {
            try
            {
                var tutorCertificates = await _context.TutorCertificates.Where(c => tutorCertificateId.Contains(c.TutorCertificateId)).ToListAsync();
                if (tutorCertificates == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not Found", "Tutor Certificate not found");
                }
                foreach (var tutorCertificate in tutorCertificates)
                {
                    tutorCertificate.IsVerified = false;
                    // Gửi mail cho tutor khi bị từ chối
                    var tutor = await _context.Tutors.FirstOrDefaultAsync(c => c.TutorId == tutorCertificate.TutorId);
                    var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == tutor.UserId);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = user.Email,
                        Subject = "Chứng chỉ bị từ chối",
                        Body = "Chứng chỉ của bạn đã bị từ chối. Vui lòng kiểm tra lại thông tin và gửi lại cho chúng tôi. Chúc bạn có một ngày tốt lành!"
                    });

                    _context.TutorCertificates.Update(tutorCertificate);
                    throw new CrudException(HttpStatusCode.OK, "OK", "Tutor Certificate is denied");
                }
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Internal Server Error", "");
            }
        }

        // Deny Tutor Experience by Moderator or Admin
        public async Task DenyTutorExperience(List<Guid> tutorExperienceId)
        {
            try
            {
                var tutorExperiences = await _context.TutorExperiences.Where(c => tutorExperienceId.Contains(c.TutorExperienceId)).ToListAsync();
                if (tutorExperiences == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not Found", "Tutor Experience not found");
                }
                foreach (var tutorExperience in tutorExperiences)
                {
                    tutorExperience.IsVerified = false;
                    // Gửi mail cho tutor khi bị từ chối
                    var tutor = await _context.Tutors.FirstOrDefaultAsync(c => c.TutorId == tutorExperience.TutorId);
                    var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == tutor.UserId);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = user.Email,
                        Subject = "Kinh nghiệm bị từ chối",
                        Body = "Kinh nghiệm của bạn đã bị từ chối. Vui lòng kiểm tra lại thông tin và gửi lại cho chúng tôi. Chúc bạn có một ngày tốt lành!"
                    });

                    _context.TutorExperiences.Update(tutorExperience);
                    throw new CrudException(HttpStatusCode.OK, "OK", "Tutor Experience is denied");
                }
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Internal Server Error", "");
            }
        }

        // Get All Tutor Action Logs
        public async Task<PageResults<TutorActionResponse>> getTutorActionResponse(PagingRequest pagingRequest)
        {
            try
            {
                var tutorActionLogs = await _context.TutorActions
               .Include(c => c.TutorNavigation)
               .Include(c => c.TutorNavigation.UserNavigation)
               .Include(c => c.ModeratorNavigation)
               .Include(c => c.ModeratorNavigation.UserNavigation)
               .ToListAsync();
                if (tutorActionLogs == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not Found", "Tutor Action Logs not found");
                }
                List<TutorActionResponse> response = new List<TutorActionResponse>();
                foreach (var tutorActionLog in tutorActionLogs)
                {
                    TutorActionResponse tutorAccountResponse = new TutorActionResponse();
                    tutorAccountResponse.TutorId = tutorActionLog.TutorId;
                    tutorAccountResponse.tutorName = tutorActionLog.TutorNavigation.UserNavigation.Name;
                    tutorAccountResponse.ModeratorId = tutorActionLog.ModeratorId;
                    tutorAccountResponse.moderatorName = tutorActionLog.ModeratorNavigation.UserNavigation.Name;
                    tutorAccountResponse.ActionType = tutorActionLog.ActionType;
                    tutorAccountResponse.CreateAt = tutorActionLog.CreateAt;
                    tutorAccountResponse.Description = tutorActionLog.Description;
                    tutorAccountResponse.TutorActionId = tutorActionLog.TutorActionId;
                    tutorAccountResponse.ReponseDate = tutorActionLog.ReponseDate;
                    tutorAccountResponse.MeetingLink = tutorActionLog.MeetingLink;
                    tutorAccountResponse.Status = tutorActionLog.Status;
                    response.Add(tutorAccountResponse);
                }
                var pagingResponse = PagingHelper<TutorActionResponse>.Paging(response, pagingRequest.Page, pagingRequest.PageSize);
                return pagingResponse;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Internal Server Error", "");
            }

        }

        // Get Tutor Need To Accept 
        public async Task<PageResults<TutorSubjectInProgressResponse>> GetAllTutorHaveSubjectInProgress(PagingRequest pagingRequest)
        {
            try
            {
                var tutors = await _context.Tutors
              .Include(c => c.UserNavigation)
              .Include(c => c.TutorSubjectsNavigation)
              .Where(c => c.TutorSubjectsNavigation.Any(s => s.Status == (int)TutorSubjectEnum.InProgress))
              .ToListAsync();
                if (tutors == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not Found", "Tutor not found");
                }
                List<TutorSubjectInProgressResponse> response = new List<TutorSubjectInProgressResponse>();
                foreach (var tutor in tutors)
                {
                    TutorSubjectInProgressResponse tutorSubjectInProgressResponse = new TutorSubjectInProgressResponse();
                    tutorSubjectInProgressResponse.TutorId = tutor.TutorId;
                    tutorSubjectInProgressResponse.TutorName = tutor.UserNavigation.Name;
                    tutorSubjectInProgressResponse.TutorAvatar = tutor.UserNavigation.ImageUrl;
                    tutorSubjectInProgressResponse.Status = tutor.Status;
                    tutorSubjectInProgressResponse.TutorEmail = tutor.UserNavigation.Email;
                    response.Add(tutorSubjectInProgressResponse);
                }
                var pagingResponse = PagingHelper<TutorSubjectInProgressResponse>.Paging(response, pagingRequest.Page, pagingRequest.PageSize);
                return pagingResponse;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Internal Server Error", "");
            }
        }

        // Get TutorSubjectNeedToAccept By TutorId
        public async Task<List<TutorSubjectPreviewAdminResponse>> GetTutorSubjectByTutorId(Guid tutorId)
        {
            try
            {
                var tutorSubjects = _context.TutorSubjects
                    .Include(c => c.SubjectNavigation)
                    .Include(c => c.TutorNavigation)
                    .Include(c => c.TutorNavigation.UserNavigation)
                    .Where(c => c.TutorId == tutorId && c.Status == (int)TutorSubjectEnum.InProgress)
                    .ToList();
                if (tutorSubjects == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not Found", "Tutor Subject not found");
                }
                List<TutorSubjectPreviewAdminResponse> response = new List<TutorSubjectPreviewAdminResponse>();
                foreach (var tutorSubject in tutorSubjects)
                {
                    TutorSubjectPreviewAdminResponse tutorSubjectPreviewAdminResponse = new TutorSubjectPreviewAdminResponse();
                    tutorSubjectPreviewAdminResponse.TutorSubjectId = tutorSubject.TutorSubjectId;
                    tutorSubjectPreviewAdminResponse.TutorId = tutorSubject.TutorId;
                    tutorSubjectPreviewAdminResponse.TutorName = tutorSubject.TutorNavigation.UserNavigation.Name;
                    tutorSubjectPreviewAdminResponse.TutorAvatar = tutorSubject.TutorNavigation.UserNavigation.ImageUrl;
                    tutorSubjectPreviewAdminResponse.SubjectName = tutorSubject.SubjectNavigation.Title;
                    response.Add(tutorSubjectPreviewAdminResponse);
                }
                return response;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Internal Server Error", "");
            }
        }

        // Accept Tutor Subject by Moderator or Admin
        public async Task AcceptTutorSubject(Guid tutorSubjectId)
        {
            try
            {
                var tutorSubject = await _context.TutorSubjects.FirstOrDefaultAsync(c => c.TutorSubjectId == tutorSubjectId);
                if (tutorSubject == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not Found", "Tutor Subject not found");
                }

                tutorSubject.Status = (int)TutorSubjectEnum.Available;
                // Gửi mail cho tutor khi được accept 
                var tutor = await _context.Tutors.FirstOrDefaultAsync(c => c.TutorId == tutorSubject.TutorId);
                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == tutor.UserId);
                // Ghi log tutor action 
                var tutorAction = new TutorAction()
                {
                    TutorId = tutor.TutorId,
                    ModeratorId = Guid.Parse("a8267106-edfd-4057-998a-64eac4045fec"),
                    ActionType = (int)TutorActionTypeEnum.TutorRegisterSubject,
                    CreateAt = DateTime.Now,
                    Description = "Xác nhận môn học gia sư đăng kí",
                    Status = (int)TutorActionEnum.Accept,
                    ReponseDate = DateTime.Now,
                };
                await _appExtension.SendMail(new MailContent()
                {
                    To = user.Email,
                    Subject = "Môn học đã được xác nhận",
                    Body = "Môn học của bạn đã được hệ thống xác nhận và công nhận. Kiểm tra lại nhé nếu có gì sai sót liên hệ với chúng tôi nhé. Chúc bạn có một ngày tốt lành!"
                });
                _context.TutorActions.Add(tutorAction);
                _context.TutorSubjects.Update(tutorSubject);
                _context.SaveChanges();
                throw new CrudException(HttpStatusCode.OK, "OK", "Tutor Subject is accepted");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Internal Server Error", "");
            }
        }

        // Deny Tutor Subject by Moderator or Admin
        public async Task DenyTutorSubject(Guid tutorSubjectId)
        {
            try
            {
                var tutorSubject = await _context.TutorSubjects.FirstOrDefaultAsync(c => c.TutorSubjectId == tutorSubjectId);
                if (tutorSubject == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not Found", "Tutor Subject not found");
                }
                // Gửi mail cho tutor khi bị từ chối
                var tutor = await _context.Tutors.FirstOrDefaultAsync(c => c.TutorId == tutorSubject.TutorId);
                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == tutor.UserId);
                // Ghi log tutor action
                var tutorAction = new TutorAction()
                {
                    TutorId = tutor.TutorId,
                    ModeratorId = Guid.Parse("a8267106-edfd-4057-998a-64eac4045fec"),
                    ActionType = (int)TutorActionTypeEnum.TutorRegisterSubject,
                    CreateAt = DateTime.Now,
                    Description = "Từ chối môn học gia sư đăng kí",
                    Status = (int)TutorActionEnum.Reject,
                    ReponseDate = DateTime.Now,
                };
                await _appExtension.SendMail(new MailContent()
                {
                    To = user.Email,
                    Subject = "Môn học bị từ chối",
                    Body = "Môn học của bạn đã bị từ chối. Vui lòng kiểm tra lại thông tin và gửi lại cho chúng tôi. Chúc bạn có một ngày tốt lành!"
                });
                _context.TutorActions.Add(tutorAction);
                _context.TutorSubjects.Remove(tutorSubject);
                _context.SaveChanges();
                throw new CrudException(HttpStatusCode.OK, "OK", "Tutor Subject is denied");
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Internal Server Error", "");
            }
        }
        public async Task<IActionResult> GetFreeTutorSubscription() => new JsonResult(_subscriptions["mienPhi"]);
        public async Task<IActionResult> GetBasicTutorSubscription() => new JsonResult(_subscriptions["traiNghiem"]);
        public async Task<IActionResult> GetPremiumTutorSubscription() => new JsonResult(_subscriptions["thanhVien"]);
        public async Task<IActionResult> UpdateTutorSubscription(TutorSubscriptionSetting setting, int choice)
        {
            List<TutorSubscriptionSetting> tutorSubscriptionSettings = new List<TutorSubscriptionSetting>();
            TutorSubscriptionSetting subscriptionSetting = new TutorSubscriptionSetting();
            switch (choice)
            {
                case 1:
                    subscriptionSetting = _subscriptions["mienPhi"];
                    break;
                case 2:
                    subscriptionSetting = _subscriptions["traiNghiem"];
                    break;
                case 3:
                    subscriptionSetting = _subscriptions["thanhVien"];
                    break;
                default:
                    return new StatusCodeResult(400);
            }
            subscriptionSetting.Price = setting.Price;

            await SaveTutorSubscriptionsAsync();
            return new JsonResult(subscriptionSetting);
        }
        private async Task SaveTutorSubscriptionsAsync()
        {
            var json = JsonConvert.SerializeObject(_subscriptions, Formatting.Indented);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tutorSubscription.json");
            await File.WriteAllTextAsync(path, json);
        }
        public async Task<IActionResult> GetFreeStudentSubscription() => new JsonResult(_studentSubscriptions["mienPhi"]);
        public async Task<IActionResult> GetPremiumStudentSubscription() => new JsonResult(_studentSubscriptions["thanhVien"]);
        public async Task<IActionResult> UpdateStudentSubscription(StudentSubscriptionSetting setting, int choice)
        {
            List<StudentSubscriptionSetting> studentSubscriptionSettings = new List<StudentSubscriptionSetting>();
            StudentSubscriptionSetting subscriptionSetting = new StudentSubscriptionSetting();
            switch (choice)
            {
                case 1:
                    subscriptionSetting = _studentSubscriptions["mienPhi"];
                    break;
                case 2:
                    subscriptionSetting = _studentSubscriptions["thanhVien"];
                    break;
                default:
                    return new StatusCodeResult(400);
            }
            subscriptionSetting.Price = setting.Price;
            subscriptionSetting.RequestCreatePerDay = setting.RequestCreatePerDay;
            subscriptionSetting.ContactPerDay = setting.ContactPerDay;

            await SaveStudentSubscriptionsAsync();
            return new JsonResult(subscriptionSetting);
        }
        private async Task SaveStudentSubscriptionsAsync()
        {
            var json = JsonConvert.SerializeObject(_subscriptions, Formatting.Indented);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "studentSubscription.json");
            await File.WriteAllTextAsync(path, json);
        }

        // Get All Tutor Subscription and Paging 
        public async Task<PageResults<TutorSubscriptionSetting>> GetAllTutorSubscription(PagingRequest pagingRequest)
        {
            try
            {
                List<TutorSubscriptionSetting> tutorSubscriptionSettings = new List<TutorSubscriptionSetting>();
                foreach (var subscription in _subscriptions)
                {
                    var setting = subscription.Value;
                    setting.Name = subscription.Key;
                    tutorSubscriptionSettings.Add(setting);
                }
                var pagingResponse = PagingHelper<TutorSubscriptionSetting>.Paging(tutorSubscriptionSettings, pagingRequest.Page, pagingRequest.PageSize);
                return pagingResponse;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Internal Server Error", "");
            }
        }

        // Create New Tutor Subscription
        public async Task<IActionResult> CreateTutorSubscription(TutorSubscriptionRequest setting)
        {
            try
            {
                // Thêm cái mutalDescription và Private Description vào đây
                List<String> mutualDescriptions = new List<String>();
                List<String> privateDescriptions = new List<String>();
                foreach ( var textString in setting.MutualDescriptions)
                {
                    mutualDescriptions.Add(textString);
                }
                foreach (var textString in setting.PrivateDescriptions)
                {
                    privateDescriptions.Add(textString);
                }
                TutorSubscriptionSetting settings = new TutorSubscriptionSetting()
                {   
                    Name = GenerateRandomCode(),
                    TutorNameSubscription = setting.TutorNameSubscription,
                    Price = setting.Price,
                    Type = setting.Types,
                    CreatedAt = DateTime.Now,
                    Status = (Int32)TutorSubscriptionStatusEnum.Inactive,
                    MutualDescriptions = mutualDescriptions,
                    PrivateDescriptions = privateDescriptions
                };
                _subscriptions.Add(settings.Name, settings);
                await SaveTutorSubscriptionsAsync();
                return new JsonResult(settings);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Update Status Tutor Subscription Inactive or Active 
        public async Task<IActionResult> UpdateTutorSubscriptionStatus(string name)
        {
            try
            {
                if (!_subscriptions.ContainsKey(name))
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not Found", "Tutor Subscription not found");
                }
                if (_subscriptions[name].Status == (Int32)TutorSubscriptionStatusEnum.Active)
                {
                    _subscriptions[name].Status = (Int32)TutorSubscriptionStatusEnum.Inactive;
                }
                else
                {
                    _subscriptions[name].Status = (Int32)TutorSubscriptionStatusEnum.Active;
                }
                await SaveTutorSubscriptionsAsync();
                // Trả về mã trạng thái OK với thông báo thành công
                return new JsonResult(_subscriptions[name]);
            }
            catch (Exception ex)
            {
                // Trả về mã trạng thái InternalServerError với thông báo lỗi
                throw new CrudException(HttpStatusCode.InternalServerError, "Internal Server Error", "");
            }
        }

        // Update Tutor Subscription
        public async Task<IActionResult> UpdateTutorSubscription(TutorSubscriptionRequest setting, string name)
        {
            try
            {
                if (!_subscriptions.ContainsKey(name))
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not Found", "Tutor Subscription not found");
                }
                _subscriptions[name].TutorNameSubscription = setting.TutorNameSubscription;
                _subscriptions[name].Price = setting.Price;
                _subscriptions[name].Type = setting.Types;
                _subscriptions[name].MutualDescriptions = setting.MutualDescriptions;
                _subscriptions[name].PrivateDescriptions = setting.PrivateDescriptions;
                await SaveTutorSubscriptionsAsync();
                return new JsonResult(_subscriptions[name]);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Internal Server Error", "");
            }
        }

        // Remove Tutor Subscription
        public async Task<IActionResult> RemoveTutorSubscription(string name)
        {
            try
            {
                if (!_subscriptions.ContainsKey(name))
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not Found", "Tutor Subscription not found");
                }
                _subscriptions.Remove(name);
                await SaveTutorSubscriptionsAsync();
                return new StatusCodeResult(200);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.InternalServerError, "Internal Server Error", "");
            }
        }

        // Generate Tutor Subscription
        private static string GenerateRandomCode()
        {
            Random random = new Random();
            int firstNumber = random.Next(100, 1000); // Số ngẫu nhiên từ 100 đến 999
            int secondNumber = random.Next(100, 1000); // Số ngẫu nhiên từ 100 đến 999

            return $"#SUB{firstNumber}{secondNumber}";
        }

    }
}
