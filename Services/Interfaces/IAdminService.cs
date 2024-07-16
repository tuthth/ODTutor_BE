using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAdminService
    {
        Task<ActionResult<List<User>>> GetAllUsers();
        Task<ActionResult<PageResults<User>>> GetAllUsersPaging(PagingRequest request);
        Task<ActionResult<User>> GetUser(Guid id);
        Task<ActionResult<List<Tutor>>> GetAllTutors();
        Task<ActionResult<Tutor>> GetTutor(Guid id);
        Task<ActionResult<List<TutorCertificate>>> GetAllTutorCertificates();
        Task<ActionResult<TutorCertificate>> GetTutorCertificate(Guid id);
        Task<ActionResult<List<TutorCertificate>>> GetTutorCertificatesByTutorId(Guid id);
        Task<ActionResult<List<TutorCertificate>>> GetTutorCertificatesBySubjectId(Guid id);
        Task<ActionResult<List<TutorSubject>>> GetAllTutorSubjects();
        Task<ActionResult<TutorSubject>> GetTutorSubject(Guid id);
        Task<ActionResult<List<TutorSubject>>> GetTutorSubjectsByTutorId(Guid id);
        Task<ActionResult<List<TutorSubject>>> GetTutorSubjectsBySubjectId(Guid id);
        Task<ActionResult<List<TutorRating>>> GetAllTutorRatings();
        Task<ActionResult<TutorRating>> GetTutorRating(Guid id);
        Task<ActionResult<List<TutorRating>>> GetTutorRatingsByTutorId(Guid id);
        Task<ActionResult<List<TutorRating>>> GetTutorRatingsByStudentId(Guid id);
        Task<ActionResult<List<TutorRatingImage>>> GetAllTutorRatingImages();
        Task<ActionResult<TutorRatingImage>> GetTutorRatingImage(Guid id);
        Task<ActionResult<List<Moderator>>> GetModerators();
        Task<ActionResult<Moderator>> GetModeratorById(Guid id);
        Task<ActionResult<List<Notification>>> GetNotifications();
        Task<ActionResult<List<Notification>>> GetNotificationsByUserId(Guid id);
        Task<IActionResult> GetStudentStatisticsByDayOfWeek();
        Task<IActionResult> GetStudentStatisticsByMonth();
        Task<IActionResult> GetBookingStatisticsByMonth();
        Task<IActionResult> GetBookingTransactionStatisticsByMonth();
        Task<IActionResult> GetBookingStatisticsOf1TutorByMonth(Guid tutorId);
        Task<IActionResult> GetBookingStatisticsTop5TutorsByMonth();
        Task<IActionResult> GetBookingTransactionStatisticsOfATutorByMonth(Guid receiverId);
        Task<IActionResult> GetBookingTransactionStatisticsTop5TutorsByMonth();
    }
}
