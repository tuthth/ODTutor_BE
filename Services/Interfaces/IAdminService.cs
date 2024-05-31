﻿using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
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
        Task<ActionResult<User>> GetUser(Guid id);
        Task<ActionResult<List<Student>>> GetAllStudents();
        Task<ActionResult<Student>> GetStudent(Guid id);
        Task<ActionResult<List<Tutor>>> GetAllTutors();
        Task<ActionResult<Tutor>> GetTutor(Guid id);
        Task<ActionResult<List<Subject>>> GetAllSubjects();
        Task<ActionResult<Subject>> GetSubject(Guid id);
        Task<ActionResult<List<CourseTransaction>>> GetAllCourseTransactions();
        Task<ActionResult<CourseTransaction>> GetCourseTransaction(Guid id);
        Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsBySenderId(Guid id);
        Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByReceiverId(Guid id);
        Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByCourseId(Guid id);
        Task<ActionResult<List<Schedule>>> GetAllSchedules();
        Task<ActionResult<Schedule>> GetSchedule(Guid id);
        Task<ActionResult<List<Schedule>>> GetSchedulesByStudentCourseId(Guid id);
        Task<ActionResult<List<StudentCourse>>> GetAllStudentCourses();
        Task<ActionResult<StudentCourse>> GetStudentCourse(Guid id);
        Task<ActionResult<List<StudentCourse>>> GetStudentCoursesByCourseId(Guid id);
        Task<ActionResult<List<StudentCourse>>> GetStudentCoursesByStudentId(Guid id);
        Task<ActionResult<List<StudentRequest>>> GetAllStudentRequests();
        Task<ActionResult<StudentRequest>> GetStudentRequest(Guid id);
        Task<ActionResult<List<StudentRequest>>> GetStudentRequestsByStudentId(Guid id);
        Task<ActionResult<List<StudentRequest>>> GetStudentRequestsBySubjectId(Guid id);
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
        Task<ActionResult<List<TutorSchedule>>> GetAllTutorSchedules();
        Task<ActionResult<TutorSchedule>> GetTutorSchedule(Guid id);
        Task<ActionResult<List<TutorSchedule>>> GetTutorSchedulesByTutorId(Guid id);
        Task<ActionResult<List<UserBlock>>> GetAllUserBlocks();
        Task<ActionResult<List<UserBlock>>> GetAllBlockByCreateUserId(Guid id);
        Task<ActionResult<List<UserBlock>>> GetAllBlockByTargetUserId(Guid id);
        Task<ActionResult<List<UserFollow>>> GetAllUserFollows();
        Task<ActionResult<List<UserFollow>>> GetAllFollowsByCreateUserId(Guid id);
        Task<ActionResult<List<UserFollow>>> GetAllFollowsByTargetUserId(Guid id);
        Task<ActionResult<List<Wallet>>> GetAllWallets();
        Task<ActionResult<Wallet>> GetWalletByWalletId(Guid id);
        Task<ActionResult<Wallet>> GetWalletByUserId(Guid id);
        Task<ActionResult<List<BookingTransaction>>> GetAllBookingTransactions();
        Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsByBookingId(Guid id);
        Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsBySenderId(Guid id);
        Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsByReceiverId(Guid id);
        Task<ActionResult<List<WalletTransaction>>> GetAllWalletTransactions();
        Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsByWalletTransactionId(Guid id);
        Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsBySenderId(Guid id);
        Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsByReceiverId(Guid id);
        Task<ActionResult<List<Moderator>>> GetModerators();
        Task<ActionResult<Moderator>> GetModeratorById(Guid id);
        Task<ActionResult<List<Notification>>> GetNotifications();
        Task<ActionResult<List<Notification>>> GetNotificationsByUserId(Guid id);
    }
}
