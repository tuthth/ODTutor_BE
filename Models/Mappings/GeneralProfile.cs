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
            CreateMap<Wallet, WalletView>();
            CreateMap<TutorInformationRequest, Tutor>();
            CreateMap<AccountRegisterRequest, User>();
            CreateMap<User, AccountResponse>();
            CreateMap<User, UserAccountResponse>();
            CreateMap<TutorExperienceRequest,TutorExperience>();
            CreateMap<TutorDateAvailable, TutorRegistDate>();
            CreateMap<SubjectAddNewRequest, Subject>();
            CreateMap<UserBlock, UserInteractRequest>();
            CreateMap<UserFollow, UserInteractRequest>();
            CreateMap<StudentRequest, CreateStudentRequest>();
            CreateMap<StudentRequest, UpdateStudentRequest>();
            CreateMap<Booking, BookingRequest>();
            CreateMap<Booking, UpdateBookingRequest>();
            CreateMap<User, AccountRegisterRequest>();
            CreateMap<Course, CourseRequest>();
            CreateMap<Course, UpdateCourseRequest>();
            CreateMap<CourseOutline, CourseOutlineRequest>();
            CreateMap<CourseOutline, UpdateCourseOutlineRequest>();
            CreateMap<CoursePromotion, CoursePromotionRequest>();
            CreateMap<Promotion, CreatePromotion>();
            CreateMap<Promotion, UpdatePromotion>();
            CreateMap<Report, ReportRequest>();
            CreateMap<Report, UpdateReportRequest>();
            CreateMap<BookingTransaction, BookingTransactionCreate>();
            CreateMap<CourseTransaction, CourseTransactionCreate>();
            CreateMap<WalletTransaction, WalletTransactionCreate>();
            CreateMap<TutorExperience, TutorExperienceRequest>();
            CreateMap<TutorRating, TutorRatingRequest>();
            CreateMap<TutorRating, UpdateTutorRatingRequest>();
            CreateMap<User, UpdateAccountRequest>();
            CreateMap<User, UpdateUserAccountRequest>();
        }
    }
}
