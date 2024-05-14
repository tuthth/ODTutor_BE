USE [master]
GO
/****** Object:  Database [ODTutor]    Script Date: 14-May-24 13:24:41 ******/
CREATE DATABASE [ODTutor]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ODTutor', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLSERVER\MSSQL\DATA\ODTutor.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ODTutor_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLSERVER\MSSQL\DATA\ODTutor_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [ODTutor] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ODTutor].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ODTutor] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ODTutor] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ODTutor] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ODTutor] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ODTutor] SET ARITHABORT OFF 
GO
ALTER DATABASE [ODTutor] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ODTutor] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ODTutor] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ODTutor] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ODTutor] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ODTutor] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ODTutor] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ODTutor] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ODTutor] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ODTutor] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ODTutor] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ODTutor] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ODTutor] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ODTutor] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ODTutor] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ODTutor] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ODTutor] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ODTutor] SET RECOVERY FULL 
GO
ALTER DATABASE [ODTutor] SET  MULTI_USER 
GO
ALTER DATABASE [ODTutor] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ODTutor] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ODTutor] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ODTutor] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ODTutor] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [ODTutor] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'ODTutor', N'ON'
GO
ALTER DATABASE [ODTutor] SET QUERY_STORE = ON
GO
ALTER DATABASE [ODTutor] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [ODTutor]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bookings]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bookings](
	[BookingId] [uniqueidentifier] NOT NULL,
	[StudentId] [uniqueidentifier] NOT NULL,
	[TutorId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[Duration] [time](7) NOT NULL,
	[ActualEndTime] [datetime2](7) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[TotalPrice] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[GoogleMeetUrl] [nvarchar](max) NULL,
 CONSTRAINT [PK_Bookings] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingTransactions]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingTransactions](
	[BookingTransactionId] [uniqueidentifier] NOT NULL,
	[SenderWalletId] [uniqueidentifier] NOT NULL,
	[BookingId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[ReceiverWalletId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_BookingTransactions] PRIMARY KEY CLUSTERED 
(
	[BookingTransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CourseOutlines]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourseOutlines](
	[CourseOutlineId] [uniqueidentifier] NOT NULL,
	[CourseId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_CourseOutlines] PRIMARY KEY CLUSTERED 
(
	[CourseOutlineId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CoursePromotions]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CoursePromotions](
	[PromotionId] [uniqueidentifier] NOT NULL,
	[CourseId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_CoursePromotions] PRIMARY KEY CLUSTERED 
(
	[PromotionId] ASC,
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Courses]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Courses](
	[CourseId] [uniqueidentifier] NOT NULL,
	[TutorId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[TotalMoney] [decimal](18, 2) NOT NULL,
	[TotalSlots] [int] NOT NULL,
	[Note] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Courses] PRIMARY KEY CLUSTERED 
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CourseTransactions]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourseTransactions](
	[CourseTransactionId] [uniqueidentifier] NOT NULL,
	[SenderWalletId] [uniqueidentifier] NOT NULL,
	[CourseId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[ReceiverWalletId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CourseTransactions] PRIMARY KEY CLUSTERED 
(
	[CourseTransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Promotions]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Promotions](
	[PromotionId] [uniqueidentifier] NOT NULL,
	[PromotionCode] [nvarchar](max) NULL,
	[Percentage] [decimal](18, 2) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Promotions] PRIMARY KEY CLUSTERED 
(
	[PromotionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reports]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reports](
	[ReportId] [uniqueidentifier] NOT NULL,
	[SenderUserId] [uniqueidentifier] NOT NULL,
	[TargetId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[Content] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Reports] PRIMARY KEY CLUSTERED 
(
	[ReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Schedules]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schedules](
	[ScheduleId] [uniqueidentifier] NOT NULL,
	[StudentCourseId] [uniqueidentifier] NOT NULL,
	[StartAt] [datetime2](7) NOT NULL,
	[EndAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Schedules] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentCourses]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentCourses](
	[StudentCourseId] [uniqueidentifier] NOT NULL,
	[CourseId] [uniqueidentifier] NOT NULL,
	[StudentId] [uniqueidentifier] NOT NULL,
	[Status] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_StudentCourses] PRIMARY KEY CLUSTERED 
(
	[StudentCourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentRequests]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentRequests](
	[StudentRequestId] [uniqueidentifier] NOT NULL,
	[StudentId] [uniqueidentifier] NOT NULL,
	[SubjectId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_StudentRequests] PRIMARY KEY CLUSTERED 
(
	[StudentRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Students]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Students](
	[StudentId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED 
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subjects]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subjects](
	[SubjectId] [uniqueidentifier] NOT NULL,
	[ImageUrl] [nvarchar](max) NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[Note] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Subjects] PRIMARY KEY CLUSTERED 
(
	[SubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TutorCertificates]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TutorCertificates](
	[TutorCertificateId] [uniqueidentifier] NOT NULL,
	[TutorId] [uniqueidentifier] NOT NULL,
	[TutorSubjectId] [uniqueidentifier] NOT NULL,
	[ImageUrl] [nvarchar](max) NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_TutorCertificates] PRIMARY KEY CLUSTERED 
(
	[TutorCertificateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TutorRatingImages]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TutorRatingImages](
	[TutorRatingImageId] [uniqueidentifier] NOT NULL,
	[TutorId] [uniqueidentifier] NOT NULL,
	[TutorRatingId] [uniqueidentifier] NOT NULL,
	[ImageUrl] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_TutorRatingImages] PRIMARY KEY CLUSTERED 
(
	[TutorRatingImageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TutorRatings]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TutorRatings](
	[TutorRatingId] [uniqueidentifier] NOT NULL,
	[TutorId] [uniqueidentifier] NOT NULL,
	[StudentId] [uniqueidentifier] NOT NULL,
	[RatePoints] [int] NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[BookingId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TutorRatings] PRIMARY KEY CLUSTERED 
(
	[TutorRatingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tutors]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tutors](
	[TutorId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[IdentityNumber] [nvarchar](max) NOT NULL,
	[Level] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[PricePerHour] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_Tutors] PRIMARY KEY CLUSTERED 
(
	[TutorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TutorSchedules]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TutorSchedules](
	[TutorScheduleId] [uniqueidentifier] NOT NULL,
	[TutorId] [uniqueidentifier] NOT NULL,
	[StartTime] [datetime2](7) NOT NULL,
	[ActualEndTime] [datetime2](7) NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_TutorSchedules] PRIMARY KEY CLUSTERED 
(
	[TutorScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TutorSubjects]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TutorSubjects](
	[TutorSubjectId] [uniqueidentifier] NOT NULL,
	[TutorId] [uniqueidentifier] NOT NULL,
	[SubjectId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_TutorSubjects] PRIMARY KEY CLUSTERED 
(
	[TutorSubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserAuthentications]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAuthentications](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[EmailToken] [nvarchar](max) NULL,
	[EmailTokenExpiry] [datetime2](7) NULL,
 CONSTRAINT [PK_UserAuthentications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserBlocks]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserBlocks](
	[CreateUserId] [uniqueidentifier] NOT NULL,
	[TargetUserId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_UserBlocks] PRIMARY KEY CLUSTERED 
(
	[CreateUserId] ASC,
	[TargetUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserFollows]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserFollows](
	[CreateUserId] [uniqueidentifier] NOT NULL,
	[TargetUserId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_UserFollows] PRIMARY KEY CLUSTERED 
(
	[CreateUserId] ASC,
	[TargetUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[Username] [nvarchar](max) NULL,
	[ImageUrl] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[Banned] [bit] NOT NULL,
	[BanExpiredAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Wallets]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wallets](
	[WalletId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Amount] [decimal](18, 2) NULL,
	[AvalaibleAmount] [decimal](18, 2) NULL,
	[PendingAmount] [decimal](18, 2) NULL,
	[LastBalanceUpdate] [datetime2](7) NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Wallets] PRIMARY KEY CLUSTERED 
(
	[WalletId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WalletTransactions]    Script Date: 14-May-24 13:24:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WalletTransactions](
	[SenderWalletId] [uniqueidentifier] NOT NULL,
	[BookingId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[WalletTransactionId] [uniqueidentifier] NOT NULL,
	[ReceiverWalletId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_WalletTransactions] PRIMARY KEY CLUSTERED 
(
	[WalletTransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_StudentId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_StudentId] ON [dbo].[Bookings]
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_TutorId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_TutorId] ON [dbo].[Bookings]
(
	[TutorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BookingTransactions_BookingId]    Script Date: 14-May-24 13:24:42 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_BookingTransactions_BookingId] ON [dbo].[BookingTransactions]
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BookingTransactions_ReceiverWalletId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_BookingTransactions_ReceiverWalletId] ON [dbo].[BookingTransactions]
(
	[ReceiverWalletId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BookingTransactions_SenderWalletId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_BookingTransactions_SenderWalletId] ON [dbo].[BookingTransactions]
(
	[SenderWalletId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CourseOutlines_CourseId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_CourseOutlines_CourseId] ON [dbo].[CourseOutlines]
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CoursePromotions_CourseId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_CoursePromotions_CourseId] ON [dbo].[CoursePromotions]
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Courses_TutorId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_Courses_TutorId] ON [dbo].[Courses]
(
	[TutorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CourseTransactions_ReceiverWalletId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_CourseTransactions_ReceiverWalletId] ON [dbo].[CourseTransactions]
(
	[ReceiverWalletId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CourseTransactions_SenderWalletId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_CourseTransactions_SenderWalletId] ON [dbo].[CourseTransactions]
(
	[SenderWalletId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Reports_SenderUserId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_Reports_SenderUserId] ON [dbo].[Reports]
(
	[SenderUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Schedules_StudentCourseId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_Schedules_StudentCourseId] ON [dbo].[Schedules]
(
	[StudentCourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StudentCourses_CourseId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_StudentCourses_CourseId] ON [dbo].[StudentCourses]
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StudentCourses_StudentId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_StudentCourses_StudentId] ON [dbo].[StudentCourses]
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StudentRequests_StudentId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_StudentRequests_StudentId] ON [dbo].[StudentRequests]
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StudentRequests_SubjectId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_StudentRequests_SubjectId] ON [dbo].[StudentRequests]
(
	[SubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Students_UserId]    Script Date: 14-May-24 13:24:42 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Students_UserId] ON [dbo].[Students]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorCertificates_TutorId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_TutorCertificates_TutorId] ON [dbo].[TutorCertificates]
(
	[TutorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorCertificates_TutorSubjectId]    Script Date: 14-May-24 13:24:42 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_TutorCertificates_TutorSubjectId] ON [dbo].[TutorCertificates]
(
	[TutorSubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorRatingImages_TutorId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_TutorRatingImages_TutorId] ON [dbo].[TutorRatingImages]
(
	[TutorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorRatingImages_TutorRatingId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_TutorRatingImages_TutorRatingId] ON [dbo].[TutorRatingImages]
(
	[TutorRatingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorRatings_BookingId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_TutorRatings_BookingId] ON [dbo].[TutorRatings]
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorRatings_StudentId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_TutorRatings_StudentId] ON [dbo].[TutorRatings]
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorRatings_TutorId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_TutorRatings_TutorId] ON [dbo].[TutorRatings]
(
	[TutorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Tutors_UserId]    Script Date: 14-May-24 13:24:42 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tutors_UserId] ON [dbo].[Tutors]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorSchedules_TutorId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_TutorSchedules_TutorId] ON [dbo].[TutorSchedules]
(
	[TutorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorSubjects_SubjectId]    Script Date: 14-May-24 13:24:42 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_TutorSubjects_SubjectId] ON [dbo].[TutorSubjects]
(
	[SubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TutorSubjects_TutorId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_TutorSubjects_TutorId] ON [dbo].[TutorSubjects]
(
	[TutorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserAuthentications_UserId]    Script Date: 14-May-24 13:24:42 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserAuthentications_UserId] ON [dbo].[UserAuthentications]
(
	[UserId] ASC
)
WHERE ([UserId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserBlocks_TargetUserId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_UserBlocks_TargetUserId] ON [dbo].[UserBlocks]
(
	[TargetUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserFollows_TargetUserId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_UserFollows_TargetUserId] ON [dbo].[UserFollows]
(
	[TargetUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Wallets_UserId]    Script Date: 14-May-24 13:24:42 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Wallets_UserId] ON [dbo].[Wallets]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_WalletTransactions_ReceiverWalletId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_WalletTransactions_ReceiverWalletId] ON [dbo].[WalletTransactions]
(
	[ReceiverWalletId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_WalletTransactions_SenderWalletId]    Script Date: 14-May-24 13:24:42 ******/
CREATE NONCLUSTERED INDEX [IX_WalletTransactions_SenderWalletId] ON [dbo].[WalletTransactions]
(
	[SenderWalletId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BookingTransactions] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [ReceiverWalletId]
GO
ALTER TABLE [dbo].[CourseTransactions] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [ReceiverWalletId]
GO
ALTER TABLE [dbo].[Tutors] ADD  DEFAULT ((0.0)) FOR [PricePerHour]
GO
ALTER TABLE [dbo].[WalletTransactions] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [WalletTransactionId]
GO
ALTER TABLE [dbo].[WalletTransactions] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [ReceiverWalletId]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_Students_StudentId] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([StudentId])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_Students_StudentId]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_Tutors_TutorId] FOREIGN KEY([TutorId])
REFERENCES [dbo].[Tutors] ([TutorId])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_Tutors_TutorId]
GO
ALTER TABLE [dbo].[BookingTransactions]  WITH CHECK ADD  CONSTRAINT [FK_BookingTransactions_Bookings_BookingId] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([BookingId])
GO
ALTER TABLE [dbo].[BookingTransactions] CHECK CONSTRAINT [FK_BookingTransactions_Bookings_BookingId]
GO
ALTER TABLE [dbo].[BookingTransactions]  WITH CHECK ADD  CONSTRAINT [FK_BookingTransactions_Wallets_ReceiverWalletId] FOREIGN KEY([ReceiverWalletId])
REFERENCES [dbo].[Wallets] ([WalletId])
GO
ALTER TABLE [dbo].[BookingTransactions] CHECK CONSTRAINT [FK_BookingTransactions_Wallets_ReceiverWalletId]
GO
ALTER TABLE [dbo].[BookingTransactions]  WITH CHECK ADD  CONSTRAINT [FK_BookingTransactions_Wallets_SenderWalletId] FOREIGN KEY([SenderWalletId])
REFERENCES [dbo].[Wallets] ([WalletId])
GO
ALTER TABLE [dbo].[BookingTransactions] CHECK CONSTRAINT [FK_BookingTransactions_Wallets_SenderWalletId]
GO
ALTER TABLE [dbo].[CourseOutlines]  WITH CHECK ADD  CONSTRAINT [FK_CourseOutlines_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([CourseId])
GO
ALTER TABLE [dbo].[CourseOutlines] CHECK CONSTRAINT [FK_CourseOutlines_Courses_CourseId]
GO
ALTER TABLE [dbo].[CoursePromotions]  WITH CHECK ADD  CONSTRAINT [FK_CoursePromotions_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([CourseId])
GO
ALTER TABLE [dbo].[CoursePromotions] CHECK CONSTRAINT [FK_CoursePromotions_Courses_CourseId]
GO
ALTER TABLE [dbo].[CoursePromotions]  WITH CHECK ADD  CONSTRAINT [FK_CoursePromotions_Promotions_PromotionId] FOREIGN KEY([PromotionId])
REFERENCES [dbo].[Promotions] ([PromotionId])
GO
ALTER TABLE [dbo].[CoursePromotions] CHECK CONSTRAINT [FK_CoursePromotions_Promotions_PromotionId]
GO
ALTER TABLE [dbo].[Courses]  WITH CHECK ADD  CONSTRAINT [FK_Courses_Tutors_TutorId] FOREIGN KEY([TutorId])
REFERENCES [dbo].[Tutors] ([TutorId])
GO
ALTER TABLE [dbo].[Courses] CHECK CONSTRAINT [FK_Courses_Tutors_TutorId]
GO
ALTER TABLE [dbo].[CourseTransactions]  WITH CHECK ADD  CONSTRAINT [FK_CourseTransactions_Courses_CourseTransactionId] FOREIGN KEY([CourseTransactionId])
REFERENCES [dbo].[Courses] ([CourseId])
GO
ALTER TABLE [dbo].[CourseTransactions] CHECK CONSTRAINT [FK_CourseTransactions_Courses_CourseTransactionId]
GO
ALTER TABLE [dbo].[CourseTransactions]  WITH CHECK ADD  CONSTRAINT [FK_CourseTransactions_Wallets_ReceiverWalletId] FOREIGN KEY([ReceiverWalletId])
REFERENCES [dbo].[Wallets] ([WalletId])
GO
ALTER TABLE [dbo].[CourseTransactions] CHECK CONSTRAINT [FK_CourseTransactions_Wallets_ReceiverWalletId]
GO
ALTER TABLE [dbo].[CourseTransactions]  WITH CHECK ADD  CONSTRAINT [FK_CourseTransactions_Wallets_SenderWalletId] FOREIGN KEY([SenderWalletId])
REFERENCES [dbo].[Wallets] ([WalletId])
GO
ALTER TABLE [dbo].[CourseTransactions] CHECK CONSTRAINT [FK_CourseTransactions_Wallets_SenderWalletId]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_Users_SenderUserId] FOREIGN KEY([SenderUserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_Users_SenderUserId]
GO
ALTER TABLE [dbo].[Schedules]  WITH CHECK ADD  CONSTRAINT [FK_Schedules_StudentCourses_StudentCourseId] FOREIGN KEY([StudentCourseId])
REFERENCES [dbo].[StudentCourses] ([StudentCourseId])
GO
ALTER TABLE [dbo].[Schedules] CHECK CONSTRAINT [FK_Schedules_StudentCourses_StudentCourseId]
GO
ALTER TABLE [dbo].[StudentCourses]  WITH CHECK ADD  CONSTRAINT [FK_StudentCourses_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([CourseId])
GO
ALTER TABLE [dbo].[StudentCourses] CHECK CONSTRAINT [FK_StudentCourses_Courses_CourseId]
GO
ALTER TABLE [dbo].[StudentCourses]  WITH CHECK ADD  CONSTRAINT [FK_StudentCourses_Students_StudentId] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([StudentId])
GO
ALTER TABLE [dbo].[StudentCourses] CHECK CONSTRAINT [FK_StudentCourses_Students_StudentId]
GO
ALTER TABLE [dbo].[StudentRequests]  WITH CHECK ADD  CONSTRAINT [FK_StudentRequests_Students_StudentId] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([StudentId])
GO
ALTER TABLE [dbo].[StudentRequests] CHECK CONSTRAINT [FK_StudentRequests_Students_StudentId]
GO
ALTER TABLE [dbo].[StudentRequests]  WITH CHECK ADD  CONSTRAINT [FK_StudentRequests_Subjects_SubjectId] FOREIGN KEY([SubjectId])
REFERENCES [dbo].[Subjects] ([SubjectId])
GO
ALTER TABLE [dbo].[StudentRequests] CHECK CONSTRAINT [FK_StudentRequests_Subjects_SubjectId]
GO
ALTER TABLE [dbo].[Students]  WITH CHECK ADD  CONSTRAINT [FK_Students_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Students] CHECK CONSTRAINT [FK_Students_Users_UserId]
GO
ALTER TABLE [dbo].[TutorCertificates]  WITH CHECK ADD  CONSTRAINT [FK_TutorCertificates_Tutors_TutorId] FOREIGN KEY([TutorId])
REFERENCES [dbo].[Tutors] ([TutorId])
GO
ALTER TABLE [dbo].[TutorCertificates] CHECK CONSTRAINT [FK_TutorCertificates_Tutors_TutorId]
GO
ALTER TABLE [dbo].[TutorCertificates]  WITH CHECK ADD  CONSTRAINT [FK_TutorCertificates_TutorSubjects_TutorSubjectId] FOREIGN KEY([TutorSubjectId])
REFERENCES [dbo].[TutorSubjects] ([TutorSubjectId])
GO
ALTER TABLE [dbo].[TutorCertificates] CHECK CONSTRAINT [FK_TutorCertificates_TutorSubjects_TutorSubjectId]
GO
ALTER TABLE [dbo].[TutorRatingImages]  WITH CHECK ADD  CONSTRAINT [FK_TutorRatingImages_TutorRatings_TutorRatingId] FOREIGN KEY([TutorRatingId])
REFERENCES [dbo].[TutorRatings] ([TutorRatingId])
GO
ALTER TABLE [dbo].[TutorRatingImages] CHECK CONSTRAINT [FK_TutorRatingImages_TutorRatings_TutorRatingId]
GO
ALTER TABLE [dbo].[TutorRatingImages]  WITH CHECK ADD  CONSTRAINT [FK_TutorRatingImages_Tutors_TutorId] FOREIGN KEY([TutorId])
REFERENCES [dbo].[Tutors] ([TutorId])
GO
ALTER TABLE [dbo].[TutorRatingImages] CHECK CONSTRAINT [FK_TutorRatingImages_Tutors_TutorId]
GO
ALTER TABLE [dbo].[TutorRatings]  WITH CHECK ADD  CONSTRAINT [FK_TutorRatings_Bookings_BookingId] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([BookingId])
GO
ALTER TABLE [dbo].[TutorRatings] CHECK CONSTRAINT [FK_TutorRatings_Bookings_BookingId]
GO
ALTER TABLE [dbo].[TutorRatings]  WITH CHECK ADD  CONSTRAINT [FK_TutorRatings_Students_StudentId] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([StudentId])
GO
ALTER TABLE [dbo].[TutorRatings] CHECK CONSTRAINT [FK_TutorRatings_Students_StudentId]
GO
ALTER TABLE [dbo].[TutorRatings]  WITH CHECK ADD  CONSTRAINT [FK_TutorRatings_Tutors_TutorId] FOREIGN KEY([TutorId])
REFERENCES [dbo].[Tutors] ([TutorId])
GO
ALTER TABLE [dbo].[TutorRatings] CHECK CONSTRAINT [FK_TutorRatings_Tutors_TutorId]
GO
ALTER TABLE [dbo].[Tutors]  WITH CHECK ADD  CONSTRAINT [FK_Tutors_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Tutors] CHECK CONSTRAINT [FK_Tutors_Users_UserId]
GO
ALTER TABLE [dbo].[TutorSchedules]  WITH CHECK ADD  CONSTRAINT [FK_TutorSchedules_Tutors_TutorId] FOREIGN KEY([TutorId])
REFERENCES [dbo].[Tutors] ([TutorId])
GO
ALTER TABLE [dbo].[TutorSchedules] CHECK CONSTRAINT [FK_TutorSchedules_Tutors_TutorId]
GO
ALTER TABLE [dbo].[TutorSubjects]  WITH CHECK ADD  CONSTRAINT [FK_TutorSubjects_Subjects_SubjectId] FOREIGN KEY([SubjectId])
REFERENCES [dbo].[Subjects] ([SubjectId])
GO
ALTER TABLE [dbo].[TutorSubjects] CHECK CONSTRAINT [FK_TutorSubjects_Subjects_SubjectId]
GO
ALTER TABLE [dbo].[TutorSubjects]  WITH CHECK ADD  CONSTRAINT [FK_TutorSubjects_Tutors_TutorId] FOREIGN KEY([TutorId])
REFERENCES [dbo].[Tutors] ([TutorId])
GO
ALTER TABLE [dbo].[TutorSubjects] CHECK CONSTRAINT [FK_TutorSubjects_Tutors_TutorId]
GO
ALTER TABLE [dbo].[UserAuthentications]  WITH CHECK ADD  CONSTRAINT [FK_UserAuthentications_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserAuthentications] CHECK CONSTRAINT [FK_UserAuthentications_Users_UserId]
GO
ALTER TABLE [dbo].[UserBlocks]  WITH CHECK ADD  CONSTRAINT [FK_UserBlocks_Users_CreateUserId] FOREIGN KEY([CreateUserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserBlocks] CHECK CONSTRAINT [FK_UserBlocks_Users_CreateUserId]
GO
ALTER TABLE [dbo].[UserBlocks]  WITH CHECK ADD  CONSTRAINT [FK_UserBlocks_Users_TargetUserId] FOREIGN KEY([TargetUserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserBlocks] CHECK CONSTRAINT [FK_UserBlocks_Users_TargetUserId]
GO
ALTER TABLE [dbo].[UserFollows]  WITH CHECK ADD  CONSTRAINT [FK_UserFollows_Users_CreateUserId] FOREIGN KEY([CreateUserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserFollows] CHECK CONSTRAINT [FK_UserFollows_Users_CreateUserId]
GO
ALTER TABLE [dbo].[UserFollows]  WITH CHECK ADD  CONSTRAINT [FK_UserFollows_Users_TargetUserId] FOREIGN KEY([TargetUserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserFollows] CHECK CONSTRAINT [FK_UserFollows_Users_TargetUserId]
GO
ALTER TABLE [dbo].[Wallets]  WITH CHECK ADD  CONSTRAINT [FK_Wallets_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Wallets] CHECK CONSTRAINT [FK_Wallets_Users_UserId]
GO
ALTER TABLE [dbo].[WalletTransactions]  WITH CHECK ADD  CONSTRAINT [FK_WalletTransactions_Wallets_ReceiverWalletId] FOREIGN KEY([ReceiverWalletId])
REFERENCES [dbo].[Wallets] ([WalletId])
GO
ALTER TABLE [dbo].[WalletTransactions] CHECK CONSTRAINT [FK_WalletTransactions_Wallets_ReceiverWalletId]
GO
ALTER TABLE [dbo].[WalletTransactions]  WITH CHECK ADD  CONSTRAINT [FK_WalletTransactions_Wallets_SenderWalletId] FOREIGN KEY([SenderWalletId])
REFERENCES [dbo].[Wallets] ([WalletId])
GO
ALTER TABLE [dbo].[WalletTransactions] CHECK CONSTRAINT [FK_WalletTransactions_Wallets_SenderWalletId]
GO
USE [master]
GO
ALTER DATABASE [ODTutor] SET  READ_WRITE 
GO
