using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class ODTutorContext : DbContext
    {
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingTransaction> BookingTransactions { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseOutline> CourseOutlines { get; set; }
        public DbSet<CoursePromotion> CoursePromotions { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use your preferred connection string here
            optionsBuilder.UseSqlServer("server=(local);user=sa;password=12345;database=ODTutor;Trusted_Connection=True;TrustServerCertificate=True");
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

    // BookingTransaction Entity Configuration
    modelBuilder.Entity<BookingTransaction>()
        .HasKey(bt => bt.BookingTransactionId);

    modelBuilder.Entity<BookingTransaction>()
        .HasOne(bt => bt.WalletNavigation)
        .WithMany(w => w.BookingTransactionsNavigation)
        .HasForeignKey(bt => bt.WalletId).OnDelete(DeleteBehavior.NoAction);

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

    // CourseOutline Entity Configuration
    modelBuilder.Entity<CourseOutline>()
        .HasKey(co => co.CourseOutlineId);

    modelBuilder.Entity<CourseOutline>()
        .HasOne(co => co.CoursesNavigation)
        .WithMany(c => c.CourseOutlinesNavigation)
        .HasForeignKey(co => co.CourseId).OnDelete(DeleteBehavior.NoAction);

    // CoursePromotion Entity Configuration
    modelBuilder.Entity<CoursePromotion>()
        .HasKey(cp => cp.PromotionId);

    modelBuilder.Entity<CoursePromotion>()
        .HasOne(cp => cp.CourseNavigation)
        .WithMany(c => c.CoursePromotionsNavigation)
        .HasForeignKey(cp => cp.CourseId).OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<CoursePromotion>()
        .HasMany(cp => cp.PromotionsNavigation)
        .WithMany(p => p.CoursePromotionsNavigations)
        .UsingEntity(j => j.ToTable("CoursePromotionPromotions"));

    // Promotion Entity Configuration
    modelBuilder.Entity<Promotion>()
        .HasKey(p => p.PromotionId);

    modelBuilder.Entity<Promotion>()
        .HasMany(p => p.CoursePromotionsNavigations)
        .WithMany(cp => cp.PromotionsNavigation)
        .UsingEntity(j => j.ToTable("CoursePromotionPromotions"));

    // Report Entity Configuration
    modelBuilder.Entity<Report>()
        .HasKey(r => r.ReportId);

    modelBuilder.Entity<Report>()
        .HasOne(r => r.UserNavigation)
        .WithMany(u => u.SenderUserReportNavigation)
        .HasForeignKey(r => r.SenderUserId).OnDelete(DeleteBehavior.NoAction);

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

    modelBuilder.Entity<Subject>()
        .HasOne(subj => subj.TutorSubjectNavigation)
        .WithOne(ts => ts.SubjectNavigation)
        .HasForeignKey<TutorSubject>(ts => ts.TutorSubjectId).OnDelete(DeleteBehavior.NoAction);

    // Tutor Entity Configuration
    modelBuilder.Entity<Tutor>()
        .HasKey(t => t.TutorId);

    modelBuilder.Entity<Tutor>()
        .HasOne(t => t.UserNavigation)
        .WithOne(u => u.TutorNavigation)
        .HasForeignKey<Tutor>(t => t.UserId).OnDelete(DeleteBehavior.NoAction);

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
        .HasMany(t => t.TutorRatingsImagesNavigation)
        .WithOne(tri => tri.TutorNavigation)
        .HasForeignKey(tri => tri.TutorId).OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<Tutor>()
        .HasMany(t => t.BookingsNavigation)
        .WithOne(b => b.TutorNavigation)
        .HasForeignKey(b => b.TutorId).OnDelete(DeleteBehavior.NoAction);

    // TutorCertificate Entity Configuration
    modelBuilder.Entity<TutorCertificate>()
        .HasKey(tc => tc.TutorCertificateId);

    modelBuilder.Entity<TutorCertificate>()
        .HasOne(tc => tc.TutorNavigation)
        .WithMany(t => t.TutorCertificatesNavigation)
        .HasForeignKey(tc => tc.TutorId).OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<TutorCertificate>()
        .HasOne(tc => tc.TutorSubjectNavigation)
        .WithOne(ts => ts.TutorCertificateNavigation)
        .HasForeignKey<TutorCertificate>(tc => tc.TutorSubjectId).OnDelete(DeleteBehavior.NoAction);

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
        .HasOne(tri => tri.TutorNavigation)
        .WithMany(t => t.TutorRatingsImagesNavigation)
        .HasForeignKey(tri => tri.TutorId).OnDelete(DeleteBehavior.NoAction);

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
        .WithOne(subj => subj.TutorSubjectNavigation)
        .HasForeignKey<TutorSubject>(ts => ts.SubjectId).OnDelete(DeleteBehavior.NoAction);

    // User Entity Configuration
    modelBuilder.Entity<User>()
        .HasKey(u => u.Id);

    modelBuilder.Entity<User>()
        .HasOne(u => u.UserAuthenticationNavigation)
        .WithOne(ua => ua.UserNavigation)
        .HasForeignKey<UserAuthentication>(ua => ua.UserId).OnDelete(DeleteBehavior.NoAction);

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
        .WithOne(u => u.UserAuthenticationNavigation)
        .HasForeignKey<UserAuthentication>(ua => ua.UserId).OnDelete(DeleteBehavior.NoAction);

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
        .HasMany(w => w.BookingTransactionsNavigation)
        .WithOne(bt => bt.WalletNavigation)
        .HasForeignKey(bt => bt.WalletId).OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<Wallet>()
        .HasMany(w => w.WalletTransactionsNavigation)
        .WithOne(wt => wt.WalletNavigation)
        .HasForeignKey(wt => wt.WalletId).OnDelete(DeleteBehavior.NoAction);

    // WalletTransaction Entity Configuration
    modelBuilder.Entity<WalletTransaction>()
        .HasKey(wt => wt.WalletId);

    modelBuilder.Entity<WalletTransaction>()
        .HasOne(wt => wt.WalletNavigation)
        .WithMany(w => w.WalletTransactionsNavigation)
        .HasForeignKey(wt => wt.WalletId).OnDelete(DeleteBehavior.NoAction);
}

    }
}
