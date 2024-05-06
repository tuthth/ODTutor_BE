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
        Task<IActionResult> CreateDepositVnPay(TransactionCreate transactionCreate, Guid userId);
        Task<BookingTransaction> GetBookingTransactionById(Guid transactionId);
        Task UpdateBookingTransactionInfoInDatabase(BookingTransaction transaction);
        Task<WalletTransaction> GetWalletTransactionById(Guid transactionId);
        Task UpdateWalletTransactionInfoInDatabase(WalletTransaction transaction);
    }
}
