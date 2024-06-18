using AutoMapper;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Models.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<AccountRegisterRequest, User>();
            CreateMap<Booking, BookingRequest>();
            CreateMap<Booking, BookingView>();
            CreateMap<Booking, UpdateBookingRequest>();
            CreateMap<BookingTransaction, BookingTransactionCreate>();
            CreateMap<BookingTransaction, BookingTransactionView>();
            CreateMap<Course, CourseRequest>();
            CreateMap<Course, CourseView>();
            CreateMap<Course, UpdateCourseRequest>();
            CreateMap<CourseOutline, CourseOutlineRequest>();
            CreateMap<CourseOutline, CourseOutlineView>();
            CreateMap<CourseOutline, UpdateCourseOutlineRequest>();
            CreateMap<CoursePromotion, CoursePromotionRequest>();
            CreateMap<CoursePromotion, CoursePromotionView>();
            CreateMap<CourseTransaction, CourseTransactionCreate>();
            CreateMap<CourseTransaction, CourseTransactionView>();
            CreateMap<LoginGoogleRequest, User>();
            CreateMap<Moderator, ModeratorView>();
            CreateMap<Promotion, CreatePromotion>();
            CreateMap<Promotion, PromotionView>();
            CreateMap<Promotion, UpdatePromotion>();
            CreateMap<Report, ReportRequest>();
            CreateMap<Report, ReportView>();
            CreateMap<Report, UpdateReportRequest>();
            CreateMap<Schedule, ScheduleView>();
            CreateMap<Student, StudentView>();
            CreateMap<StudentCourse, StudentCourseView>();
            CreateMap<StudentRequest, CreateStudentRequest>();
            CreateMap<StudentRequest, StudentRequestView>();
            CreateMap<StudentRequest, UpdateStudentRequest>();
            CreateMap<Subject, SubjectAddNewRequest>();
            CreateMap<Subject, SubjectView>();
            CreateMap<Subject, UpdateSubject>();
            CreateMap<TutorCertificate, TutorCertificateView>();
            CreateMap<TutorDateAvailable, TutorRegistDate>();
            CreateMap<TutorExperience, TutorExperienceRequest>();
            CreateMap<TutorExperience, TutorExperienceView>();
            CreateMap<TutorExperience, UpdateTutorExperienceRequest>();
            CreateMap<TutorExperienceRequest, TutorExperience>();
            CreateMap<TutorInformationRequest, Tutor>();
            CreateMap<TutorSubInformationRequest, Tutor>();
            CreateMap<Tutor, TutorView>();
            CreateMap<TutorRating, TutorRatingRequest>();
            CreateMap<TutorRating, TutorRatingView>();
            CreateMap<TutorRatingImage, TutorRatingImageView>();
            CreateMap<TutorSubject, TutorSubjectView>();
            CreateMap<TutorSubject, TutorSubInformationRequest>();
            //CreateMap<TutorSubject, TutorSubjectUpdateRequest>();
            CreateMap<UserBlock, UserBlockView>();
            CreateMap<UserBlock, UserInteractRequest>();
            CreateMap<UserFollow, UserFollowView>();
            CreateMap<UserFollow, UserInteractRequest>();
        }

    }
}
