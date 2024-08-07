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
    public interface ITransactionService
    {
        Task<IActionResult> CreateDepositVnPayBooking(BookingTransactionCreate transactionCreate);
        Task<IActionResult> CreateDepositVnPayCourse(CourseTransactionCreate transactionCreate);
        Task<IActionResult> UpgradeAccount(WalletTransactionCreate transactionCreate);
        Task<IActionResult> CreateDepositToAccount(WalletTransactionCreate transactionCreate);
        Task<IActionResult> UpdateTransaction(Guid id, int choice, int status);
        Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionByAccountId(Guid id);
        Task<ActionResult<PageResults<WalletTransaction>>> GetWalletTransactionByAccountIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<List<BookingTransaction>>> GetAllBookingTransactions();
        Task<ActionResult<PageResults<BookingTransaction>>> GetAllBookingTransactionsPaging(PagingRequest request);
        Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsByBookingId(Guid id);
        Task<ActionResult<PageResults<BookingTransaction>>> GetBookingTransactionsByBookingIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsBySenderId(Guid id);
        Task<ActionResult<PageResults<BookingTransaction>>> GetBookingTransactionsBySenderIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsByReceiverId(Guid id);
        Task<ActionResult<PageResults<BookingTransaction>>> GetBookingTransactionsByReceiverIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<List<WalletTransaction>>> GetAllWalletTransactions();
        Task<ActionResult<PageResults<WalletTransaction>>> GetAllWalletTransactionsPaging(PagingRequest request);
        Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsByWalletTransactionId(Guid id);
        Task<ActionResult<PageResults<WalletTransaction>>> GetWalletTransactionsByWalletTransactionIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionByWalletId(Guid id);
        Task<ActionResult<PageResults<WalletTransaction>>> GetWalletTransactionByWalletIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsBySenderId(Guid id);
        Task<ActionResult<PageResults<WalletTransaction>>> GetWalletTransactionsBySenderIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsByReceiverId(Guid id);
        Task<ActionResult<PageResults<WalletTransaction>>> GetWalletTransactionsByReceiverIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<List<CourseTransaction>>> GetAllCourseTransactions();
        Task<ActionResult<PageResults<CourseTransaction>>> GetAllCourseTransactionsPaging(PagingRequest request);
        Task<ActionResult<CourseTransaction>> GetCourseTransaction(Guid id);
        Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsBySenderId(Guid id);
        Task<ActionResult<PageResults<CourseTransaction>>> GetCourseTransactionBySenderIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByReceiverId(Guid id);
        Task<ActionResult<PageResults<CourseTransaction>>> GetCourseTransactionsByReceiverIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByCourseId(Guid id);
        Task<ActionResult<PageResults<CourseTransaction>>> GetCourseTransactionsByCourseIdPaging(Guid id, PagingRequest request);
        Task<ActionResult<BookingTransaction>> GetBookingTransactionByBookingId(Guid bookingId);
        Task<IActionResult> HasBoughtTutorExperiencePackage(WalletTransactionCreate request);
        Task<IActionResult> UpdateTutorBackNormalTutor(Guid tutorId);
        Task<IActionResult> HasBoughtTutorPackage(WalletTransactionCreate request);
        Task<ActionResult<PageResults<WalletTransactionViewVersion2>>> GetCourseTransactionsByUserIdPaging(Guid userId, PagingRequest request);
        Task<ActionResult> CheckAllBookingFinish();
        Task<ActionResult<List<WalletTransaction>>> GetSubscriptionTransactionsOfWalletId(Guid walletId);
        Task<ActionResult<List<WalletTransaction>>> GetAllSubscriptionTransactions();
    }
}
