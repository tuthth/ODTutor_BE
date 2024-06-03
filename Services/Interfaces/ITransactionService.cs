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
        Task<BookingTransaction> GetBookingTransactionById(Guid transactionId);
        Task<CourseTransaction> GetCourseTransactionById(Guid courseId);
        Task<WalletTransaction> GetWalletTransactionById(Guid transactionId);
        Task<IActionResult> UpdateTransaction(Guid id, int choice, int status);
        Task UpdateBookingTransactionInfoInDatabase(BookingTransaction transaction);
        Task UpdateCourseTransactionInfoInDatabase(CourseTransaction transaction);
        Task UpdateWalletTransactionInfoInDatabase(WalletTransaction transaction);
        Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionByAccountId();
        Task<List<BookingTransaction>> GetAllBooking();
        Task<List<CourseTransaction>> GetAllCourse();
        Task<List<WalletTransaction>> GetAllWallet();

    }
}
