using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Settings.VNPay;
using Services.Interfaces;
using Models.Models.Requests;
using Models.Entities;
using Models.Enumerables;
using Microsoft.EntityFrameworkCore;

namespace Services.Implementations
{
    public class TransactionService : BaseService, ITransactionService
    {
        private readonly VNPaySetting _vnPaySetting;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TransactionService(ODTutorContext _context, IMapper mapper, IOptions<VNPaySetting> options, IHttpContextAccessor httpContextAccessor) : base(_context, mapper)
        {
            _vnPaySetting = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> CreateDepositVnPay(TransactionCreate transactionCreate, Guid userId)
        {
            var wallet = _context.Wallets.Include(w => w.BookingTransactionsNavigation).FirstOrDefault(w => w.UserId.Equals(userId));
            if (wallet == null)
            {
                return new StatusCodeResult(404);
            }
            BookingTransaction transaction = new BookingTransaction()
            {
                Amount = transactionCreate.Amount,
                WalletId = wallet.WalletId,
                Status = (int)VNPayType.PENDING,
            };
            _context.BookingTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            string vnp_Returnurl = transactionCreate.RedirectUrl;
            string vnp_Url = _vnPaySetting.VnPay_Url.ToString();
            string vnp_TmnCode = _vnPaySetting.VnPay_TmnCode.ToString();
            string vnp_HashSecret = _vnPaySetting.VnPay_HashSecret.ToString();

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Math.Floor(decimal.Parse(transactionCreate.Amount.ToString()) * 100).ToString());
            vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(_httpContextAccessor));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng: " + transaction.BookingId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", transaction.BookingTransactionId.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return new JsonResult(new
            {
                PaymentUrl = paymentUrl
            });
        }

        public async Task<BookingTransaction> GetBookingTransactionById(Guid transactionId)
        {
            var tran = await _context.BookingTransactions.Include(t => t.WalletNavigation).FirstOrDefaultAsync(t => t.BookingTransactionId == transactionId);
            if (tran == null)
            {
                return null;
            }
            return tran;
        }

        public async Task UpdateBookingTransactionInfoInDatabase(BookingTransaction transaction)
        {
            _context.BookingTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<WalletTransaction> GetWalletTransactionById(Guid transactionId)
        {
            var tran = await _context.WalletTransactions.Include(t => t.WalletNavigation).FirstOrDefaultAsync(t => t.WalletTransactionId == transactionId);
            if (tran == null)
            {
                return null;
            }
            return tran;
        }

        public async Task UpdateWalletTransactionInfoInDatabase(WalletTransaction transaction)
        {
            _context.WalletTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
