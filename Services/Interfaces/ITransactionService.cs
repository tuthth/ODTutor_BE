using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Requests;
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
        Task UpdateBookingTransactionInfoInDatabase(BookingTransaction transaction);
        Task UpdateCourseTransactionInfoInDatabase(CourseTransaction transaction);
        Task UpdateWalletTransactionInfoInDatabase(WalletTransaction transaction);
        Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionByAccountId();
        Task<List<BookingTransaction>> GetAllBooking();
        Task<List<CourseTransaction>> GetAllCourse();
        Task<List<WalletTransaction>> GetAllWallet();
        Task<ActionResult<List<BookingTransaction>>> GetAllBookingTransactions();
        Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsByBookingId(Guid id);
        Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsBySenderId(Guid id);
        Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsByReceiverId(Guid id);
        Task<ActionResult<List<WalletTransaction>>> GetAllWalletTransactions();
        Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsByWalletTransactionId(Guid id);
        Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsBySenderId(Guid id);
        Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsByReceiverId(Guid id);
        Task<ActionResult<List<CourseTransaction>>> GetAllCourseTransactions();
        Task<ActionResult<CourseTransaction>> GetCourseTransaction(Guid id);
        Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsBySenderId(Guid id);
        Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByReceiverId(Guid id);
        Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByCourseId(Guid id);
    }
}
