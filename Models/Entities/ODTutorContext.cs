using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class ODTutorContext : DbContext
    {
        public ODTutorContext()
        {
        }
        public ODTutorContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingTransaction> BookingTransactions { get; set; }
        public DbSet<Course> Courses {get; set;}
        public DbSet<CourseOutline> CourseOutlines { get; set; }
        public DbSet<CoursePromotion> CoursePromotions { get; set; }
        public DbSet<CourseSlot> CourseSlots { get; set; }
        public DbSet<CourseSchedule> CourseSchedules { get; set; }
        public DbSet<CourseTransaction> CourseTransactions { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportImages> ReportImages { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentRequest> StudentRequests { get; set; }
        public DbSet<Tutor> Tutors { get; set; }
        public DbSet<TutorCertificate> TutorCertificates { get; set; }
        public DbSet<TutorRating> TutorRatings { get; set; }
        public DbSet<TutorRatingImage> TutorRatingImages { get; set; }
        public DbSet<TutorSubject> TutorSubjects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAuthentication> UserAuthentications { get; set; }
        public DbSet<UserBlock> UserBlocks { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<Moderator> Moderators { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TutorExperience> TutorExperiences { get; set; }
        public DbSet<TutorAction> TutorActions { get; set; }
        public DbSet<TutorWeekAvailable> TutorWeekAvailables { get; set; }
        public DbSet<TutorDateAvailable> TutorDateAvailables { get; set; }
        public DbSet<TutorSlotAvailable> TutorSlotAvailables { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use your preferred connection string here
            optionsBuilder.UseSqlServer(GetConnectionStrings()).EnableSensitiveDataLogging();
            //optionsBuilder.UseSqlServer("Server=14.225.205.28;uid=sa;pwd=Abc@123123;Database=ODTutor;Encrypt=false;TrustServerCertificate=true;");
            //optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=12345;Database=ODTutor;Encrypt=false;TrustServerCertificate=true;");
            //optionsBuilder.UseSqlServer(GetConnectionStrings());
            //optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=12345;Database=ODTutor;Encrypt=false;TrustServerCertificate=true;");
        }

        private string GetConnectionStrings()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();
            return config.GetConnectionString("DefaultConnection");
        }

        // Configure entity relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Booking Entity Configuration
            modelBuilder.Entity<Booking>()
                .HasKey(b => b.BookingId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.StudentNavigation)
                .WithMany(s => s.BookingsNavigation)
                .HasForeignKey(b => b.StudentId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.TutorNavigation)
                .WithMany(t => t.BookingsNavigation)
                .HasForeignKey(b => b.TutorId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.BookingTransactionNavigation)
                .WithOne(bt => bt.BookingNavigation)
                .HasForeignKey<BookingTransaction>(bt => bt.BookingId).OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.TutorSubjectNavigation)
                .WithMany(ts => ts.BookingNavigation)
                .HasForeignKey(b => b.TutorSubjectId).OnDelete(DeleteBehavior.NoAction);

            // BookingTransaction Entity Configuration
            modelBuilder.Entity<BookingTransaction>()
                .HasKey(bt => bt.BookingTransactionId);

            modelBuilder.Entity<BookingTransaction>()
                .HasOne(bt => bt.SenderWalletNavigation)
                .WithMany(w => w.SenderBookingTransactionsNavigation)
                .HasForeignKey(bt => bt.SenderWalletId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BookingTransaction>()
                .HasOne(bt => bt.ReceiverWalletNavigation)
                .WithMany(w => w.ReceiverBookingTransactionsNavigation)
                .HasForeignKey(bt => bt.ReceiverWalletId).OnDelete(DeleteBehavior.NoAction);

            // Course Entity Configuration
            modelBuilder.Entity<Course>()
                .HasKey(c => c.CourseId);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.TutorNavigation)
                .WithMany(t => t.CoursesNavigation)
                .HasForeignKey(c => c.TutorId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.CourseOutlinesNavigation)
                .WithOne(co => co.CoursesNavigation)
                .HasForeignKey(co => co.CourseId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.CoursePromotionsNavigation)
                .WithOne(cp => cp.CourseNavigation)
                .HasForeignKey(cp => cp.CourseId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.StudentCoursesNavigation)
                .WithOne(sc => sc.CourseNavigation)
                .HasForeignKey(sc => sc.CourseId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Course>().HasOne(c => c.CourseTransactionNavigation)
                .WithOne(ct => ct.CourseNavigation)
                .HasForeignKey<CourseTransaction>(ct => ct.CourseTransactionId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.CourseSlotsNavigation)
                .WithOne(cs => cs.CourseNavigation)
                .HasForeignKey(cs => cs.CourseId).OnDelete(DeleteBehavior.NoAction);


            //CourseTransaction Entity Configuration
            modelBuilder.Entity<CourseTransaction>()
                .HasKey(ct => ct.CourseTransactionId);

            modelBuilder.Entity<CourseTransaction>()
                .HasOne(ct => ct.SenderWalletNavigation)
                .WithMany(w => w.SenderCourseTransactionsNavigation)
                .HasForeignKey(ct => ct.SenderWalletId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CourseTransaction>()
                .HasOne(ct => ct.ReceiverWalletNavigation)
                .WithMany(w => w.ReceiverCourseTransactionsNavigation)
                .HasForeignKey(ct => ct.ReceiverWalletId).OnDelete(DeleteBehavior.NoAction);

            // CourseOutline Entity Configuration
            modelBuilder.Entity<CourseOutline>()
                .HasKey(co => co.CourseOutlineId);

            modelBuilder.Entity<CourseOutline>()
                .HasOne(co => co.CoursesNavigation)
                .WithMany(c => c.CourseOutlinesNavigation)
                .HasForeignKey(co => co.CourseId).OnDelete(DeleteBehavior.NoAction);

            // CoursePromotion Entity Configuration
            modelBuilder.Entity<CoursePromotion>()
           .HasKey(cp => new { cp.PromotionId, cp.CourseId });


            modelBuilder.Entity<CoursePromotion>()
                .HasOne(cp => cp.CourseNavigation)
                .WithMany(c => c.CoursePromotionsNavigation)
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<CoursePromotion>()
                .HasOne(cp => cp.PromotionNavigation)
                .WithMany(p => p.CoursePromotionsNavigation)
                .HasForeignKey(cp => cp.PromotionId)
                .OnDelete(DeleteBehavior.NoAction);

            // Promotion Entity Configuration
            modelBuilder.Entity<Promotion>()
                .HasKey(p => p.PromotionId);

            modelBuilder.Entity<Promotion>()
                .HasOne(p => p.TutorNavigation)
                .WithMany(t => t.PromotionsNavigation)
                .HasForeignKey(p => p.TutorId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Promotion>()
                .HasMany(p => p.CoursePromotionsNavigation)
                .WithOne(cp => cp.PromotionNavigation)
                .HasForeignKey(cp => cp.PromotionId)
                .OnDelete(DeleteBehavior.NoAction);

            // Report Entity Configuration
            modelBuilder.Entity<Report>()
                .HasKey(r => r.ReportId);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.UserNavigation)
                .WithMany(u => u.SenderUserReportNavigation)
                .HasForeignKey(r => r.SenderUserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Report>()
                .HasMany(r => r.ReportImages)
                .WithOne(ri => ri.ReportNavigation)
                .HasForeignKey(ri => ri.ReportId).OnDelete(DeleteBehavior.NoAction);
            // ReprotImages Entity Configuration
            modelBuilder.Entity<ReportImages>()
                .HasKey(ri => ri.ReportImageId);

            // Schedule Entity Configuration
            modelBuilder.Entity<Schedule>()
                .HasKey(s => s.ScheduleId);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.StudentCourseNavigation)
                .WithMany(sc => sc.SchedulesNavigations)
                .HasForeignKey(s => s.StudentCourseId).OnDelete(DeleteBehavior.NoAction);

            // Student Entity Configuration
            modelBuilder.Entity<Student>()
                .HasKey(s => s.StudentId);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.UserNavigation)
                .WithOne(u => u.StudentNavigation)
                .HasForeignKey<Student>(s => s.UserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Student>()
                .HasMany(s => s.BookingsNavigation)
                .WithOne(b => b.StudentNavigation)
                .HasForeignKey(b => b.StudentId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Student>()
                .HasMany(s => s.TutorRatingsNavigation)
                .WithOne(tr => tr.StudentNavigation)
                .HasForeignKey(tr => tr.StudentId).OnDelete(DeleteBehavior.NoAction);


            // StudentCourse Entity Configuration
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => sc.StudentCourseId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.StudentNavigation)
                .WithMany(s => s.StudentCoursesNavigation)
                .HasForeignKey(sc => sc.StudentId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.CourseNavigation)
                .WithMany(c => c.StudentCoursesNavigation)
                .HasForeignKey(sc => sc.CourseId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentCourse>()
                .HasMany(sc => sc.SchedulesNavigations)
                .WithOne(s => s.StudentCourseNavigation)
                .HasForeignKey(s => s.StudentCourseId).OnDelete(DeleteBehavior.NoAction);

            // Subject Entity Configuration
            modelBuilder.Entity<Subject>()
                .HasKey(subj => subj.SubjectId);

            // Tutor Entity Configuration
            modelBuilder.Entity<Tutor>()
                .HasKey(t => t.TutorId);

            modelBuilder.Entity<Tutor>()
                .HasOne(t => t.UserNavigation)
                .WithOne(u => u.TutorNavigation)
                .HasForeignKey<Tutor>(t => t.UserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Tutor>()
                .HasMany(t => t.PromotionsNavigation)
                .WithOne(p => p.TutorNavigation)
                .HasForeignKey(p => p.TutorId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Tutor>()
                .HasMany(t => t.CoursesNavigation)
                .WithOne(c => c.TutorNavigation)
                .HasForeignKey(c => c.TutorId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Tutor>()
                .HasMany(t => t.TutorCertificatesNavigation)
                .WithOne(tc => tc.TutorNavigation)
                .HasForeignKey(tc => tc.TutorId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Tutor>()
                .HasMany(t => t.TutorSubjectsNavigation)
                .WithOne(ts => ts.TutorNavigation)
                .HasForeignKey(ts => ts.TutorId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Tutor>()
                .HasMany(t => t.TutorRatingsNavigation)
                .WithOne(tr => tr.TutorNavigation)
                .HasForeignKey(tr => tr.TutorId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Tutor>()
                .HasMany(t => t.BookingsNavigation)
                .WithOne(b => b.TutorNavigation)
                .HasForeignKey(b => b.TutorId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Tutor>()
                .HasMany(t => t.TutorExperiencesNavigation)
                .WithOne(te => te.TutorNavigation)
                .HasForeignKey(te => te.TutorId).OnDelete(DeleteBehavior.NoAction);
            // TutorCertificate Entity Configuration
            modelBuilder.Entity<TutorCertificate>()
                .HasKey(tc => tc.TutorCertificateId);

            modelBuilder.Entity<TutorCertificate>()
                .HasOne(tc => tc.TutorNavigation)
                .WithMany(t => t.TutorCertificatesNavigation)
                .HasForeignKey(tc => tc.TutorId).OnDelete(DeleteBehavior.NoAction);

            // TutorRating Entity Configuration
            modelBuilder.Entity<TutorRating>()
                .HasKey(tr => tr.TutorRatingId);

            modelBuilder.Entity<TutorRating>()
                .HasOne(tr => tr.TutorNavigation)
                .WithMany(t => t.TutorRatingsNavigation)
                .HasForeignKey(tr => tr.TutorId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TutorRating>()
                .HasOne(tr => tr.StudentNavigation)
                .WithMany(s => s.TutorRatingsNavigation)
                .HasForeignKey(tr => tr.StudentId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TutorRating>()
                .HasOne(tr => tr.BookingNavigation)
                .WithMany(b => b.TutorRatingsNavigation)
                .HasForeignKey(tr => tr.BookingId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TutorRating>()
                .HasMany(tr => tr.TutorRatingImagesNavigation)
                .WithOne(tri => tri.TutorRatingNavigation)
                .HasForeignKey(tri => tri.TutorRatingId).OnDelete(DeleteBehavior.NoAction);

            // TutorRatingImage Entity Configuration
            modelBuilder.Entity<TutorRatingImage>()
                .HasKey(tri => tri.TutorRatingImageId);

            modelBuilder.Entity<TutorRatingImage>()
                .HasOne(tri => tri.TutorRatingNavigation)
                .WithMany(tr => tr.TutorRatingImagesNavigation)
                .HasForeignKey(tri => tri.TutorRatingId).OnDelete(DeleteBehavior.NoAction);

            // TutorSubject Entity Configuration
            modelBuilder.Entity<TutorSubject>()
                .HasKey(ts => ts.TutorSubjectId);

            modelBuilder.Entity<TutorSubject>()
                .HasOne(ts => ts.TutorNavigation)
                .WithMany(t => t.TutorSubjectsNavigation)
                .HasForeignKey(ts => ts.TutorId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TutorSubject>()
                .HasOne(ts => ts.SubjectNavigation)
                .WithMany(subj => subj.TutorSubjectNavigation)
                .HasForeignKey(ts => ts.SubjectId)
                .OnDelete(DeleteBehavior.NoAction);


            // User Entity Configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserAuthenticationNavigation)
                .WithOne(ua => ua.UserNavigation)
                .HasForeignKey(ua => ua.UserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CreateUserBlockNavigation)
                .WithOne(ub => ub.CreateUserNavigation)
                .HasForeignKey(u => u.CreateUserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasMany(u => u.TargetUserBlockNavigation)
                .WithOne(ub => ub.TargetUserNavigation)
                .HasForeignKey(u => u.TargetUserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CreateUserFollowNavigation)
                .WithOne(uf => uf.CreateUserNavigation)
                .HasForeignKey(u => u.CreateUserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasMany(u => u.TargetUserFollowNavigation)
                .WithOne(uf => uf.TargetUserNavigation)
                .HasForeignKey(u => u.TargetUserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SenderUserReportNavigation)
                .WithOne(r => r.UserNavigation)
                .HasForeignKey(u => u.SenderUserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(u => u.StudentNavigation)
                .WithOne(s => s.UserNavigation)
                .HasForeignKey<Student>(s => s.UserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(u => u.TutorNavigation)
                .WithOne(t => t.UserNavigation)
                .HasForeignKey<Tutor>(t => t.UserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(u => u.WalletNavigation)
                .WithOne(w => w.UserNavigation)
                .HasForeignKey<Wallet>(w => w.UserId).OnDelete(DeleteBehavior.NoAction);

            // UserAuthentication Entity Configuration
            modelBuilder.Entity<UserAuthentication>()
                .HasKey(ua => ua.Id);

            modelBuilder.Entity<UserAuthentication>()
                .HasOne(ua => ua.UserNavigation)
                .WithMany(u => u.UserAuthenticationNavigation)
                .HasForeignKey(ua => ua.UserId).OnDelete(DeleteBehavior.NoAction);

            // UserBlock Entity Configuration
            modelBuilder.Entity<UserBlock>()
                .HasKey(ub => new { ub.CreateUserId, ub.TargetUserId });

            modelBuilder.Entity<UserBlock>()
                .HasOne(ub => ub.CreateUserNavigation)
                .WithMany(u => u.CreateUserBlockNavigation)
                .HasForeignKey(ub => ub.CreateUserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserBlock>()
                .HasOne(ub => ub.TargetUserNavigation)
                .WithMany(u => u.TargetUserBlockNavigation)
                .HasForeignKey(ub => ub.TargetUserId).OnDelete(DeleteBehavior.NoAction);

            // UserFollow Entity Configuration
            modelBuilder.Entity<UserFollow>()
                .HasKey(uf => new { uf.CreateUserId, uf.TargetUserId });

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.CreateUserNavigation)
                .WithMany(u => u.CreateUserFollowNavigation)
                .HasForeignKey(uf => uf.CreateUserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.TargetUserNavigation)
                .WithMany(u => u.TargetUserFollowNavigation)
                .HasForeignKey(uf => uf.TargetUserId).OnDelete(DeleteBehavior.NoAction);

            // Wallet Entity Configuration
            modelBuilder.Entity<Wallet>()
                .HasKey(w => w.WalletId);

            modelBuilder.Entity<Wallet>()
                .HasOne(w => w.UserNavigation)
                .WithOne(u => u.WalletNavigation)
                .HasForeignKey<Wallet>(w => w.UserId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Wallet>()
                .HasMany(w => w.SenderBookingTransactionsNavigation)
                .WithOne(bt => bt.SenderWalletNavigation)
                .HasForeignKey(bt => bt.SenderWalletId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Wallet>()
                .HasMany(w => w.ReceiverWalletTransactionsNavigation)
                .WithOne(wt => wt.ReceiverWalletNavigation)
                .HasForeignKey(wt => wt.ReceiverWalletId).OnDelete(DeleteBehavior.NoAction);

            // WalletTransaction Entity Configuration
            modelBuilder.Entity<WalletTransaction>()
                .HasKey(wt => wt.WalletTransactionId);

            modelBuilder.Entity<WalletTransaction>()
                .HasOne(wt => wt.SenderWalletNavigation)
                .WithMany(w => w.SenderWalletTransactionsNavigation)
                .HasForeignKey(wt => wt.SenderWalletId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<WalletTransaction>()
               .HasOne(wt => wt.ReceiverWalletNavigation)
               .WithMany(w => w.ReceiverWalletTransactionsNavigation)
               .HasForeignKey(wt => wt.ReceiverWalletId).OnDelete(DeleteBehavior.NoAction);

            //Configure StudentRequest entity

            modelBuilder.Entity<StudentRequest>()
                .HasKey(sr => sr.StudentRequestId);

            modelBuilder.Entity<StudentRequest>()
                .HasOne(sr => sr.StudentNavigation)
                .WithMany(s => s.StudentRequestsNavigation)
                .HasForeignKey(sr => sr.StudentId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentRequest>()
                .HasOne(sr => sr.SubjectNavigation)
                .WithMany(s => s.StudentRequestsNavigation)
                .HasForeignKey(subj => subj.SubjectId).OnDelete(DeleteBehavior.NoAction);

            // Configure Notification entity
            modelBuilder.Entity<Notification>()
                .HasKey(n => n.NotificationId);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.UserNavigation)
                .WithMany(u => u.NotificationNavigation)
                .HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.NoAction);

            // Configure Moderator entity
            modelBuilder.Entity<Moderator>()
                .HasKey(m => m.ModeratorId);

            modelBuilder.Entity<Moderator>()
                .HasOne(m => m.UserNavigation)
                .WithOne(u => u.ModeratorNavigation)
                .HasForeignKey<Moderator>(m => m.UserId).OnDelete(DeleteBehavior.NoAction);

            // Configure TutorExperience entity
            modelBuilder.Entity<TutorExperience>()
                .HasKey(te => te.TutorExperienceId);

            // Configure TutorAction entity
            modelBuilder.Entity<TutorAction>()
                .HasKey(ta => ta.TutorActionId);

            modelBuilder.Entity<TutorAction>()
                .HasOne(ta => ta.ModeratorNavigation)
                .WithMany(m => m.TutorActionsNavigation)
                .HasForeignKey(ta => ta.ModeratorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TutorAction>()
                .HasOne(ta => ta.TutorNavigation)
                .WithMany(t => t.TutorActionsNavigation)
                .HasForeignKey(ta => ta.TutorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure TutorWeekAvailable entity
            modelBuilder.Entity<TutorWeekAvailable>()
                .HasKey(twa => twa.TutorWeekAvailableId);

            modelBuilder.Entity<TutorWeekAvailable>()
                .HasOne(twa => twa.Tutor)
                .WithMany(t => t.TutorWeekAvailablesNavigation)
                .HasForeignKey(twa => twa.TutorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure TutorDateAvailable entity
            modelBuilder.Entity<TutorDateAvailable>()
                .HasKey(tda => tda.TutorDateAvailableID);

            modelBuilder.Entity<TutorDateAvailable>()
                .HasOne(t => t.Tutor)
                .WithMany(t => t.TutorDateAvailablesNavigation)
                .HasForeignKey(tda => tda.TutorID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TutorDateAvailable>()
                .HasOne(t => t.TutorWeekAvailable)
                .WithMany(twa => twa.TutorDateAvailables)
                .HasForeignKey(tda => tda.TutorWeekAvailableID)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure TutorSlotAvailable entity
            modelBuilder.Entity<TutorSlotAvailable>()
                .HasKey(tsa => tsa.TutorSlotAvailableID);

            modelBuilder.Entity<TutorSlotAvailable>()
                .HasOne(t => t.TutorDateAvailable)
                .WithMany(tda => tda.TutorSlotAvailables)
                .HasForeignKey(tsa => tsa.TutorDateAvailableID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TutorSlotAvailable>()
                .HasOne(t => t.Tutor)
                .WithMany(t => t.TutorSlotAvailablesNavigation)
                .HasForeignKey(tsa => tsa.TutorID)
                .OnDelete(DeleteBehavior.NoAction);

            //Configure CourseSlot entity
            modelBuilder.Entity<CourseSlot>(entity =>
            {
                entity.HasKey(cs => cs.CourseSlotId);

                entity.HasOne(cs => cs.CourseNavigation)
                    .WithMany(c => c.CourseSlotsNavigation)
                    .HasForeignKey(cs => cs.CourseId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            //Configure CourseSchedule entity
            modelBuilder.Entity<CourseSchedule>(entity =>
            {
                entity.HasKey(cs => new { cs.CourseSlotId, cs.ScheduleId });

                entity.HasOne(cs => cs.CourseSlotNavigation)
                    .WithMany(cs => cs.CourseSchedulesNavigation)
                    .HasForeignKey(cs => cs.CourseSlotId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(cs => cs.ScheduleNavigation)
                    .WithMany(s => s.CourseSchedulesNavigation)
                    .HasForeignKey(cs => cs.ScheduleId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

    }
}
