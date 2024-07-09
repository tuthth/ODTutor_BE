﻿using AutoMapper;
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
            CreateMap<TutorExperienceRequest, TutorExperience>();
            CreateMap<TutorDateAvailable, TutorRegistDate>();
            CreateMap<SubjectAddNewRequest, Subject>();
            CreateMap<UserBlock, UserInteractRequest>();
            CreateMap<UserFollow, UserInteractRequest>();
            CreateMap<UserInteractRequest, UserFollow>();
            CreateMap<UserInteractRequest, UserBlock>();
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
            CreateMap<User, UserView>();
            CreateMap<Student, StudentView>();
            CreateMap<Tutor, TutorView>();
            CreateMap<Subject, SubjectView>();
            CreateMap<Schedule, ScheduleView>();
            CreateMap<StudentCourse, StudentCourseView>();
            CreateMap<TutorCertificate, TutorCertificateView>();
            CreateMap<TutorExperience, TutorExperienceView>();
            CreateMap<TutorSubject, TutorSubjectView>();
            CreateMap<TutorRating, TutorRatingView>();
            CreateMap<TutorRatingImage, TutorRatingImageView>();
            CreateMap<Moderator, ModeratorView>();
            CreateMap<Booking, BookingView>();
            CreateMap<Course, CourseView>();
            CreateMap<CourseOutline, CourseOutlineView>();
            CreateMap<CoursePromotion, CoursePromotionView>();
            CreateMap<Report, ReportView>();
            CreateMap<Promotion, PromotionView>();
            CreateMap<UserBlock, UserBlockView>();
            CreateMap<UserFollow, UserFollowView>();
            CreateMap<StudentRequest, StudentRequestView>();
            CreateMap<BookingTransaction, BookingTransactionView>();
            CreateMap<CourseTransaction, CourseTransactionView>();
            CreateMap<WalletTransaction, WalletTransactionView>();
            CreateMap<TutorExperience, UpdateTutorExperienceRequest>();
            CreateMap<TutorExperience, TutorExperienceRequest>();
            CreateMap<TutorRating, TutorRatingRequest>();
            CreateMap<TutorRating, UpdateTutorRatingRequest>();
            CreateMap<TutorSubject, UpdateTutorSubjectRequest>();
            //CreateMap<Tutor, TutorSubInformationRequest>();
            CreateMap<TutorSubInformationRequest, Tutor>();
            CreateMap<User, LoginGoogleRequest>();
            CreateMap<LoginGoogleRequest, User>();
            CreateMap<Subject, UpdateSubject>();
            CreateMap<Subject, SubjectAddNewRequest>();
            CreateMap<TutorAction, TutorActionView>();
            CreateMap<CourseSlot, CourseSlotView>();
            CreateMap<PageResults<Wallet>, PageResults<WalletView>>();
            CreateMap<PageResults<User>, PageResults<UserView>>();
            CreateMap<PageResults<Student>, PageResults<StudentView>>();
            CreateMap<PageResults<Tutor>, PageResults<TutorView>>();
            CreateMap<PageResults<Subject>, PageResults<SubjectView>>();
            CreateMap<PageResults<Schedule>, PageResults<ScheduleView>>();
            CreateMap<PageResults<StudentCourse>, PageResults<StudentCourseView>>();
            CreateMap<PageResults<TutorCertificate>, PageResults<TutorCertificateView>>();
            CreateMap<PageResults<TutorExperience>, PageResults<TutorExperienceView>>();
            CreateMap<PageResults<TutorSubject>, PageResults<TutorSubjectView>>();
            CreateMap<PageResults<TutorRating>, PageResults<TutorRatingView>>();
            CreateMap<PageResults<TutorRatingImage>, PageResults<TutorRatingImageView>>();
            CreateMap<PageResults<Moderator>, PageResults<ModeratorView>>();
            CreateMap<PageResults<Booking>, PageResults<BookingView>>();
            CreateMap<PageResults<Course>, PageResults<CourseView>>();
            CreateMap<PageResults<CourseOutline>, PageResults<CourseOutlineView>>();
            CreateMap<PageResults<CoursePromotion>, PageResults<CoursePromotionView>>();
            CreateMap<PageResults<Report>, PageResults<ReportView>>();
            CreateMap<PageResults<Promotion>, PageResults<PromotionView>>();
            CreateMap<PageResults<UserBlock>, PageResults<UserBlockView>>();
            CreateMap<PageResults<UserFollow>, PageResults<UserFollowView>>();
            CreateMap<PageResults<StudentRequest>, PageResults<StudentRequestView>>();
            CreateMap<PageResults<BookingTransaction>, PageResults<BookingTransactionView>>();
            CreateMap<PageResults<CourseTransaction>, PageResults<CourseTransactionView>>();
            CreateMap<PageResults<WalletTransaction>, PageResults<WalletTransactionView>>();
            CreateMap<PageResults<TutorAction>, PageResults<TutorActionView>>();
            CreateMap<StudentRequest, CreateStudentRequest>();
            CreateMap<CreateStudentRequest, StudentRequest>();
            CreateMap<PageResults<CourseSlot>, PageResults<CourseSlotView>>();
            CreateMap<TutorInformationUpdate, Tutor>();
            CreateMap<NotificationDTO, Notification>();
            CreateMap<Notification, NotificationDTO>();
        }
    }
}
