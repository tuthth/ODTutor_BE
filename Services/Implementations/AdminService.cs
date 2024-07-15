using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Enumerables;
using Models.Models.Requests;
using Models.Models.Views;
using Models.PageHelper;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Services.Implementations
{
    public class AdminService : BaseService, IAdminService
    {
        public AdminService(ODTutorContext odContext, IMapper mapper) : base(odContext, mapper)
        {
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
                    StudentsPremium = students.Count(c => c.UserNavigation.IsPremium == true),
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
                    StudentsPremium = students.Count(c => c.UserNavigation.IsPremium),
                    StudentsByMonthAndTime = studentsByMonthAndTime
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




    }
}
