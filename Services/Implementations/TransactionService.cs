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
using Models.Models.Emails;

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

        public async Task<IActionResult> CreateDepositToAccount(WalletTransactionCreate transactionCreate)
        {
            var user = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var findUser = _context.Users.FirstOrDefault(u => u.Id == Guid.Parse(user));
            if (findUser == null)
            {
                return new StatusCodeResult(404);
            }
            if(transactionCreate.Choice == (Int32)VNPayTransactionType.Deposit)
            {
                var receiverWallet = _context.Wallets.Include(w => w.ReceiverWalletTransactionsNavigation.FirstOrDefault(w => w.ReceiverWalletId.Equals(transactionCreate.TargetId)));
                if(receiverWallet == null)
                {
                    return new StatusCodeResult(404);
                }
                WalletTransaction transaction = new WalletTransaction
                {
                    WalletTransactionId = Guid.NewGuid(),
                    SenderWalletId = new Guid(),
                    ReceiverWalletId = transactionCreate.TargetId,
                    CreatedAt = DateTime.UtcNow,
                    Amount = transactionCreate.Amount,
                    Status = (int)VNPayType.APPROVE,
                };
                var receiveWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.TargetId);
                receiveWallet.PendingAmount += transactionCreate.Amount;

                _context.WalletTransactions.Add(transaction);
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
                vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(_httpContextAccessor));
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng: " + transaction.WalletTransactionId);
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", transaction.WalletTransactionId.ToString());

                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                await _appExtension.SendMail(new MailContent()
                {
                    To = findUser.Email,
                    Subject = "Xác nhận giao dịch",
                    Body = "Bạn đã tạo giao dịch nạp tiền vào tài khoản với số tiền là " + transactionCreate.Amount + " VND. Vui lòng xác nhận giao dịch tại đây: " + paymentUrl
                });
                return new JsonResult(new
                {
                    PaymentUrl = paymentUrl
                });
            }
            else if(transactionCreate.Choice == (Int32)VNPayTransactionType.Withdraw)
            {
                var senderWallet = _context.Wallets.Include(w => w.SenderWalletTransactionsNavigation.FirstOrDefault(w => w.SenderWalletId.Equals(transactionCreate.TargetId)));
                if (senderWallet == null)
                {
                    return new StatusCodeResult(404);
                }
                WalletTransaction transaction = new WalletTransaction
                {
                    WalletTransactionId = Guid.NewGuid(),
                    SenderWalletId = transactionCreate.TargetId,
                    ReceiverWalletId = new Guid(),
                    CreatedAt = DateTime.UtcNow,
                    Amount = transactionCreate.Amount,
                    Status = (int)VNPayType.APPROVE,
                };
                var sendWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.TargetId);
                if(sendWallet.Amount < transactionCreate.Amount)
                {
                    return new StatusCodeResult(409);
                }
                sendWallet.PendingAmount -= transactionCreate.Amount;

                _context.WalletTransactions.Add(transaction);
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
                vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(_httpContextAccessor));
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng: " + transaction.WalletTransactionId);
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", transaction.WalletTransactionId.ToString());

                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                await _appExtension.SendMail(new MailContent()
                {
                    To = findUser.Email,
                    Subject = "Xác nhận giao dịch",
                    Body = "Bạn đã tạo giao dịch rút tiền từ tài khoản với số tiền là " + transactionCreate.Amount + " VND. Vui lòng xác nhận giao dịch tại đây: " + paymentUrl
                });

                return new JsonResult(new
                {
                    PaymentUrl = paymentUrl
                });
            }
            else if(transactionCreate.Choice == (Int32)VNPayTransactionType.Unknown)
            {
                return new StatusCodeResult(406);
            }
            await _appExtension.SendMail(new MailContent()
            {
                To = findUser.Email,
                Subject = "Xác nhận giao dịch",
                Body = "Giao dịch không hợp lệ, vui lòng kiểm tra lại thông tin."
            });
            return new StatusCodeResult(500);
        }

        public async Task<IActionResult> CreateDepositVnPayBooking(BookingTransactionCreate transactionCreate, Guid sendUserId, Guid receiverUserId)
        {
            var user = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var findUser = _context.Users.FirstOrDefault(u => u.Id == Guid.Parse(user));
            if(findUser == null)
            {
                return new StatusCodeResult(404);
            }
            var senderWallet = _context.Wallets.Include(w => w.SenderCourseTransactionsNavigation.FirstOrDefault(w => w.SenderWalletId.Equals(sendUserId)));
            var receiverWallet = _context.Wallets.Include(w => w.ReceiverCourseTransactionsNavigation.FirstOrDefault(w => w.ReceiverWalletId.Equals(receiverUserId)));
            if (senderWallet == null || receiverWallet == null)
            {
                return new StatusCodeResult(404);
            }

            BookingTransaction transaction = new BookingTransaction
            {
                BookingTransactionId = Guid.NewGuid(),
                SenderWalletId = sendUserId,
                ReceiverWalletId = receiverUserId,
                BookingId = transactionCreate.BookingId, 
                CreatedAt = DateTime.UtcNow,
                Amount = transactionCreate.Amount,
                Status = (int)VNPayType.PENDING,
            };

            WalletTransaction senderTransaction = new WalletTransaction
            {
                WalletTransactionId = transaction.BookingTransactionId,
                SenderWalletId = sendUserId,
                ReceiverWalletId = receiverUserId,
                Amount = transactionCreate.Amount,
                CreatedAt = DateTime.UtcNow,
                Status = (int)VNPayType.PENDING
            };
            var sendWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == sendUserId);
            var receiveWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == receiverUserId);
            if(sendWallet.Amount < transactionCreate.Amount)
            {
                await _appExtension.SendMail(new MailContent()
                {
                    To = findUser.Email,
                    Subject = "Xác nhận giao dịch",
                    Body = "Giao dịch booking không hợp lệ, vui lòng kiểm tra lại thông tin."
                });
                return new StatusCodeResult(409);
            }
            sendWallet.PendingAmount -= transactionCreate.Amount;
            receiveWallet.PendingAmount += transactionCreate.Amount;

            _context.BookingTransactions.Add(transaction);
            _context.WalletTransactions.Add(senderTransaction);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = findUser.Email,
                Subject = "Xác nhận giao dịch",
                Body = "Bạn đã tạo giao dịch booking với số tiền là " + transactionCreate.Amount + " VND. Vui lòng xác nhận giao dịch tại đây: " + transactionCreate.RedirectUrl
            });
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> CreateDepositVnPayCourse(CourseTransactionCreate transactionCreate, Guid sendUserId, Guid receiverUserId)
        {
            var user = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var findUser = _context.Users.FirstOrDefault(u => u.Id == Guid.Parse(user));
            if (findUser == null)
            {
                return new StatusCodeResult(404);
            }
            var senderWallet = _context.Wallets.Include(w => w.SenderCourseTransactionsNavigation.FirstOrDefault(w => w.SenderWalletId.Equals(sendUserId)));
            var receiverWallet = _context.Wallets.Include(w => w.ReceiverCourseTransactionsNavigation.FirstOrDefault(w => w.ReceiverWalletId.Equals(receiverUserId)));
            if (senderWallet == null || receiverWallet == null)
            {
                return new StatusCodeResult(404);
            }

            CourseTransaction transaction = new CourseTransaction
            {
                CourseTransactionId = Guid.NewGuid(),
                SenderWalletId = sendUserId,
                ReceiverWalletId = receiverUserId,
                CourseId = transactionCreate.CourseId,
                CreatedAt = DateTime.UtcNow,
                Amount = transactionCreate.Amount,
                Status = (int)VNPayType.PENDING,
            };

            WalletTransaction senderTransaction = new WalletTransaction
            {
                WalletTransactionId = transaction.CourseTransactionId,
                SenderWalletId = sendUserId,
                ReceiverWalletId = receiverUserId,
                Amount = transactionCreate.Amount,
                CreatedAt = DateTime.UtcNow,
                Status = (int)VNPayType.PENDING
            };
            var sendWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == sendUserId);
            var receiveWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == receiverUserId);
            if (sendWallet.Amount < transactionCreate.Amount)
            {
                await _appExtension.SendMail(new MailContent()
                {
                    To = findUser.Email,
                    Subject = "Xác nhận giao dịch",
                    Body = "Giao dịch course không hợp lệ, vui lòng kiểm tra lại thông tin."
                });
                return new StatusCodeResult(409);
            }
            sendWallet.PendingAmount -= transactionCreate.Amount;
            receiveWallet.PendingAmount += transactionCreate.Amount;

            _context.CourseTransactions.Add(transaction);
            _context.WalletTransactions.Add(senderTransaction);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = findUser.Email,
                Subject = "Xác nhận giao dịch",
                Body = "Bạn đã tạo giao dịch course với số tiền là " + transactionCreate.Amount + " VND. Vui lòng xác nhận giao dịch tại đây: " + transactionCreate.RedirectUrl
            });
            return new StatusCodeResult(201);
        }


        public async Task<BookingTransaction> GetBookingTransactionById(Guid transactionId)
        {
            var tran = await _context.BookingTransactions.Include(t => t.SenderWalletNavigation).FirstOrDefaultAsync(t => t.BookingTransactionId == transactionId);
            if (tran == null)
            {
                return null;
            }
            return tran;
        }

        public async Task<CourseTransaction> GetCourseTransactionById(Guid transactionId)
        {
            var tran = await _context.CourseTransactions.Include(t => t.SenderWalletNavigation).FirstOrDefaultAsync(t => t.CourseTransactionId == transactionId);
            if (tran == null)
            {
                return null;
            }
            return tran;
        }

        public async Task<WalletTransaction> GetWalletTransactionById(Guid transactionId)
        {
            var tran = await _context.WalletTransactions.Include(t => t.SenderWalletNavigation).FirstOrDefaultAsync(t => t.WalletTransactionId == transactionId);
            if (tran == null)
            {
                return null;
            }
            return tran;
        }
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionByAccountId()
        {
            var user = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var findUser = _context.Users.FirstOrDefault(u => u.Id == Guid.Parse(user));
            if (findUser == null)
            {
                return new StatusCodeResult(405);
            }
            var walletTransaction = await _context.WalletTransactions.Where(a => a.SenderWalletId == Guid.Parse(user) || a.ReceiverWalletId == Guid.Parse(user)).ToListAsync();
            if(walletTransaction == null)
            {
                return new StatusCodeResult(404);
            }
            return walletTransaction;
        }
        public async Task UpdateBookingTransactionInfoInDatabase(BookingTransaction transaction)
        {
            _context.BookingTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCourseTransactionInfoInDatabase(CourseTransaction transaction)
        {
            _context.CourseTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateWalletTransactionInfoInDatabase(WalletTransaction transaction)
        {
            _context.WalletTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }
        public async Task<List<BookingTransaction>> GetAllBooking() => await _context.BookingTransactions.ToListAsync();
        public async Task<List<CourseTransaction>> GetAllCourse() => await _context.CourseTransactions.ToListAsync();
        public async Task<List<WalletTransaction>> GetAllWallet() => await _context.WalletTransactions.ToListAsync();

        public async Task<IActionResult> UpdateTransaction(Guid walletTransactionId, int choice, int updateStatus)
        {
            var wallet = await _context.WalletTransactions.FirstOrDefaultAsync(w => w.WalletTransactionId == walletTransactionId);
            //only accept pending transaction
            if (wallet == null)
            {
                return new StatusCodeResult(404);
            }
            if (wallet.Status == (Int32)VNPayType.APPROVE || wallet.Status == (Int32)VNPayType.REJECT)
            {
                return new StatusCodeResult(409);
            }
            if(updateStatus == (Int32)VNPayType.APPROVE)
            {
                if (choice == (Int32)UpdateTransactionType.Booking)
                {
                    wallet.Status = (int)VNPayType.APPROVE;
                    var booking = _context.BookingTransactions.FirstOrDefault(b => b.BookingTransactionId == wallet.WalletTransactionId);
                    booking.Status = (int)VNPayType.APPROVE;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.Amount -= booking.Amount;
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow;
                    wallet.SenderWalletNavigation.PendingAmount -= booking.Amount;
                    wallet.SenderWalletNavigation.AvalaibleAmount -= booking.Amount;

                    wallet.ReceiverWalletNavigation.Amount += booking.Amount;
                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow;
                    wallet.ReceiverWalletNavigation.AvalaibleAmount += booking.Amount;
                    wallet.ReceiverWalletNavigation.PendingAmount -= booking.Amount;

                    _context.BookingTransactions.Update(booking);
                    _context.WalletTransactions.Update(wallet);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = wallet.SenderWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch booking của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = wallet.ReceiverWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch booking của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                }
                else if (choice == (Int32)UpdateTransactionType.Course)
                {
                    wallet.Status = (int)VNPayType.APPROVE;
                    var course = _context.CourseTransactions.FirstOrDefault(b => b.CourseTransactionId == wallet.WalletTransactionId);
                    course.Status = (int)VNPayType.APPROVE;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.Amount -= course.Amount;
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow;
                    wallet.SenderWalletNavigation.PendingAmount -= course.Amount;
                    wallet.SenderWalletNavigation.AvalaibleAmount -= course.Amount;

                    wallet.ReceiverWalletNavigation.Amount += course.Amount;
                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow;
                    wallet.ReceiverWalletNavigation.AvalaibleAmount += course.Amount;
                    wallet.ReceiverWalletNavigation.PendingAmount -= course.Amount;

                    _context.CourseTransactions.Update(course);
                    _context.WalletTransactions.Update(wallet);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = wallet.SenderWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch course của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = wallet.ReceiverWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch course của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                }
                else if (choice == (Int32)UpdateTransactionType.Wallet)
                {
                    wallet.Status = (int)VNPayType.APPROVE;
                    _context.WalletTransactions.Update(wallet);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = wallet.SenderWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch ví của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = wallet.ReceiverWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch ví của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                }
                else if (choice == (Int32)UpdateTransactionType.Unknown) { return new StatusCodeResult(406); }
            }
            else if(updateStatus == (Int32)VNPayType.REJECT)
            {
                if (choice == (Int32)UpdateTransactionType.Booking)
                {
                    wallet.Status = (int)VNPayType.REJECT;
                    var booking = _context.BookingTransactions.FirstOrDefault(b => b.BookingTransactionId == wallet.WalletTransactionId);
                    booking.Status = (int)VNPayType.REJECT;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow;
                    wallet.SenderWalletNavigation.PendingAmount -= booking.Amount;

                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow;
                    wallet.ReceiverWalletNavigation.PendingAmount -= booking.Amount;

                    _context.BookingTransactions.Update(booking);
                    _context.WalletTransactions.Update(wallet);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = wallet.SenderWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch booking của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = wallet.ReceiverWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch booking của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                }
                else if (choice == (Int32)UpdateTransactionType.Course)
                {
                    wallet.Status = (int)VNPayType.REJECT;
                    var course = _context.CourseTransactions.FirstOrDefault(b => b.CourseTransactionId == wallet.WalletTransactionId);
                    course.Status = (int)VNPayType.REJECT;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow;
                    wallet.SenderWalletNavigation.PendingAmount -= course.Amount;

                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow;
                    wallet.ReceiverWalletNavigation.PendingAmount -= course.Amount;

                    _context.CourseTransactions.Update(course);
                    _context.WalletTransactions.Update(wallet);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = wallet.SenderWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch course của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = wallet.ReceiverWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch course của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                }
                else if (choice == (Int32)UpdateTransactionType.Wallet)
                {
                    wallet.Status = (int)VNPayType.REJECT;
                    _context.WalletTransactions.Update(wallet);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = wallet.SenderWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch ví của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = wallet.ReceiverWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch ví của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                }
                else if (choice == (Int32)UpdateTransactionType.Unknown) { return new StatusCodeResult(406); }
            }
            else if(updateStatus == (Int32)VNPayType.PENDING)
            {
                return new StatusCodeResult(406);
            }
            
            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }

    }
}
