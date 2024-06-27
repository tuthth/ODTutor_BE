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
using NuGet.Protocol.Plugins;
using Models.Models.Views;
using Models.PageHelper;

namespace Services.Implementations
{
    public class TransactionService : BaseService, ITransactionService
    {
        private readonly VNPaySetting _vnPaySetting;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFirebaseRealtimeDatabaseService _firebaseRealtimeDatabaseService;
        public TransactionService(ODTutorContext _context, IMapper mapper, IOptions<VNPaySetting> options, IHttpContextAccessor httpContextAccessor, IFirebaseRealtimeDatabaseService firebaseRealtimeDatabaseService) : base(_context, mapper)
        {
            _vnPaySetting = options.Value;
            _httpContextAccessor = httpContextAccessor;
            _firebaseRealtimeDatabaseService = firebaseRealtimeDatabaseService;
        }

        public async Task<IActionResult> CreateDepositToAccount(WalletTransactionCreate transactionCreate)
        {
            var findUser = new User();
            if (transactionCreate.Choice == (Int32)VNPayTransactionType.Deposit)
            {
                findUser = _context.Users.FirstOrDefault(u => u.WalletNavigation.WalletId == transactionCreate.ReceiverId);
                if (findUser == null)
                {
                    return new StatusCodeResult(404);
                }
                var receiverWallet = _context.Wallets.Include(w => w.ReceiverWalletTransactionsNavigation.FirstOrDefault(w => w.ReceiverWalletId.Equals(transactionCreate.ReceiverId)));
                if (receiverWallet == null)
                {
                    return new StatusCodeResult(404);
                }
                WalletTransaction transaction = new WalletTransaction
                {
                    WalletTransactionId = Guid.NewGuid(),
                    SenderWalletId = (Guid)transactionCreate.SenderId,
                    ReceiverWalletId = (Guid)transactionCreate.ReceiverId,
                    CreatedAt = DateTime.Now,
                    Amount = transactionCreate.Amount,
                    Status = (int)VNPayType.PENDING,
                    Note = "Nạp tiền vào tài khoản"
                };
                var receiveWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.ReceiverId);
                receiveWallet.PendingAmount += transactionCreate.Amount;
                _context.Wallets.Update(receiveWallet);
                _context.WalletTransactions.Add(transaction);

                var notification = new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "Nạp tiền vào tài khoản",
                    Content = "Bạn đã nhận được một giao dịch nạp tiền vào tài khoản với số tiền là " + transactionCreate.Amount + " VND. Mã giao dịch: " + transaction.WalletTransactionId,
                    UserId = findUser.Id,
                    CreatedAt = DateTime.Now,
                    Status = (int)NotificationEnum.UnRead
                };

                _context.Notifications.Add(notification);
                _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification.NotificationId, notification);
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
                vnpay.AddRequestData("vnp_OrderInfo", transaction.WalletTransactionId.ToString());
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                await _appExtension.SendMail(new MailContent()
                {
                    To = findUser.Email,
                    Subject = "Xác nhận giao dịch",
                    Body = "Bạn đã tạo giao dịch nạp tiền vào tài khoản với số tiền là " + transactionCreate.Amount + " VND. Vui lòng xác nhận giao dịch tại đây: " + paymentUrl
                });
                return new JsonResult(new
                {
                    WalletTransactionId = transaction.WalletTransactionId,
                    PaymentUrl = paymentUrl
                });
            }
            else if (transactionCreate.Choice == (Int32)VNPayTransactionType.Withdraw)
            {
                findUser = _context.Users.FirstOrDefault(u => u.WalletNavigation.WalletId == transactionCreate.SenderId);
                if (findUser == null)
                {
                    return new StatusCodeResult(404);
                }
                var senderWallet = _context.Wallets.Include(w => w.SenderWalletTransactionsNavigation.FirstOrDefault(w => w.SenderWalletId.Equals(transactionCreate.SenderId)));
                if (senderWallet == null)
                {
                    return new StatusCodeResult(404);
                }
                WalletTransaction transaction = new WalletTransaction
                {
                    WalletTransactionId = Guid.NewGuid(),
                    SenderWalletId = (Guid)transactionCreate.SenderId,
                    ReceiverWalletId = (Guid)transactionCreate.ReceiverId, // This should be the admin's wallet ID
                    CreatedAt = DateTime.Now,
                    Amount = transactionCreate.Amount,
                    Status = (int)VNPayType.PENDING,
                    Note = "Rút tiền từ tài khoản"
                };
                var sendWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.SenderId);
                if (sendWallet.Amount < transactionCreate.Amount)
                {
                    return new StatusCodeResult(409);
                }
                sendWallet.PendingAmount -= transactionCreate.Amount; // Deduct the amount from the sender's wallet
                _context.Wallets.Update(sendWallet);
                _context.WalletTransactions.Add(transaction);

                var notification = new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "Rút tiền từ tài khoản",
                    Content = "Bạn đã tạo một giao dịch rút tiền từ tài khoản với số tiền là " + transactionCreate.Amount + " VND. Mã giao dịch: " + transaction.WalletTransactionId,
                    UserId = findUser.Id,
                    CreatedAt = DateTime.Now,
                    Status = (int)NotificationEnum.UnRead
                };
                _context.Notifications.Add(notification);
                _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification.NotificationId, notification);
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
                vnpay.AddRequestData("vnp_OrderInfo", transaction.WalletTransactionId.ToString());
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                await _appExtension.SendMail(new MailContent()
                {
                    To = findUser.Email,
                    Subject = "Xác nhận giao dịch",
                    Body = "Bạn đã tạo giao dịch rút tiền từ tài khoản với số tiền là " + transactionCreate.Amount + " VND. Vui lòng xác nhận giao dịch tại đây: " + paymentUrl
                });

                return new JsonResult(new
                {
                    WalletTransactionId = transaction.WalletTransactionId,
                    PaymentUrl = paymentUrl
                });
            }
            else if (transactionCreate.Choice == (Int32)VNPayTransactionType.Unknown)
            {
                return new StatusCodeResult(406);
            }
            await _appExtension.SendMail(new MailContent()
            {
                To = findUser.Email,
                Subject = "Xác nhận giao dịch",
                Body = "Giao dịch không hợp lệ, vui lòng kiểm tra lại thông tin."
            });
            var notificationError = new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = "Giao dịch",
                Content = "Giao dịch không hợp lệ, vui lòng kiểm tra lại thông tin.",
                UserId = findUser.Id,
                CreatedAt = DateTime.Now,
                Status = (int)NotificationEnum.UnRead
            };
            _context.Notifications.Add(notificationError);
            _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notificationError.NotificationId, notificationError);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(500);
        }

        public async Task<IActionResult> UpgradeAccount(WalletTransactionCreate transactionCreate)
        {
            if(transactionCreate.Choice != (Int32)VNPayTransactionType.Upgrade)
            {
                return new StatusCodeResult(406);
            }
            var findUser = _context.Users.FirstOrDefault(u => u.Id == transactionCreate.SenderId);
            if(findUser == null)
            {
                return new StatusCodeResult(404);
            }
            var senderWallet = _context.Wallets.Include(w => w.SenderCourseTransactionsNavigation.FirstOrDefault(w => w.SenderWalletId.Equals(transactionCreate.SenderId)));
            var receiverWallet = _context.Wallets.Include(w => w.ReceiverCourseTransactionsNavigation.FirstOrDefault(w => w.ReceiverWalletId.Equals(transactionCreate.ReceiverId)));
            if (senderWallet == null || receiverWallet == null)
            {
                return new StatusCodeResult(404);
            }
            WalletTransaction transaction = new WalletTransaction
            {
                WalletTransactionId = Guid.NewGuid(),
                SenderWalletId = (Guid)transactionCreate.SenderId,
                ReceiverWalletId = (Guid)transactionCreate.ReceiverId,
                CreatedAt = DateTime.Now,
                Amount = transactionCreate.Amount,
                Status = (int)VNPayType.APPROVE,
            };
            var receiveWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.ReceiverId);
            receiveWallet.Amount += transactionCreate.Amount;
            var sendWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.SenderId);
            sendWallet.Amount -= transactionCreate.Amount;

            _context.WalletTransactions.Add(transaction);
            _context.Wallets.Update(receiveWallet);
            _context.Wallets.Update(sendWallet);
            findUser.IsPremium = true;
            _context.Users.Update(findUser);

            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = "Nâng cấp tài khoản",
                Content = "Tài khoản",
                UserId = findUser.Id,
                CreatedAt = DateTime.Now,
                Status = (int)NotificationEnum.UnRead
            };
            _context.Notifications.Add(notification);
            _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification.NotificationId, notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = findUser.Email,
                Subject = "Nâng cấp tài khoản",
                Body = "Tài khoản của bạn đã được nâng cấp thành Premium. Hãy truy cập hệ thống để trải nghiệm đầy đủ tính năng. \nMã giao dịch: " + transaction.WalletTransactionId
            });
            return new StatusCodeResult(200);
        }

        public async Task<IActionResult> CreateDepositVnPayBooking(BookingTransactionCreate transactionCreate)
        {
            var user = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var findUser = _context.Users.FirstOrDefault(u => u.Id == transactionCreate.SenderId);
            if (findUser == null)
            {
                return new StatusCodeResult(404);
            }
            var senderWallet = _context.Wallets.Include(w => w.SenderCourseTransactionsNavigation.FirstOrDefault(w => w.SenderWalletId.Equals(transactionCreate.SenderId)));
            var receiverWallet = _context.Wallets.Include(w => w.ReceiverCourseTransactionsNavigation.FirstOrDefault(w => w.ReceiverWalletId.Equals(transactionCreate.ReceiverId)));
            if (senderWallet == null || receiverWallet == null)
            {
                return new StatusCodeResult(404);
            }

            BookingTransaction transaction = new BookingTransaction
            {
                BookingTransactionId = Guid.NewGuid(),
                SenderWalletId = (Guid)transactionCreate.SenderId,
                ReceiverWalletId = (Guid)transactionCreate.ReceiverId,
                BookingId = transactionCreate.BookingId,
                CreatedAt = DateTime.Now,
                Amount = transactionCreate.Amount,
                Status = (int)VNPayType.PENDING,
            };

            WalletTransaction senderTransaction = new WalletTransaction
            {
                WalletTransactionId = transaction.BookingTransactionId,
                SenderWalletId = (Guid)transactionCreate.SenderId,
                ReceiverWalletId = (Guid)transactionCreate.ReceiverId,
                Amount = transactionCreate.Amount,
                CreatedAt = DateTime.Now,
                Status = (int)VNPayType.PENDING,
                Note = "Giao dịch book giáo viên"
            };
            var sendWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.SenderId);
            var receiveWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.ReceiverId);
            if (sendWallet.Amount < transactionCreate.Amount)
            {
                await _appExtension.SendMail(new MailContent()
                {
                    To = findUser.Email,
                    Subject = "Xác nhận giao dịch",
                    Body = "Giao dịch booking không hợp lệ, vui lòng kiểm tra lại thông tin."
                });
                var notificationError = new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "Giao dịch booking",
                    Content = "Giao dịch booking không hợp lệ, vui lòng kiểm tra lại thông tin.",
                    UserId = findUser.Id,
                    CreatedAt = DateTime.Now,
                    Status = (int)NotificationEnum.UnRead
                };
                _context.Notifications.Add(notificationError);
                _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notificationError.NotificationId, notificationError);
                return new StatusCodeResult(409);
            }
            sendWallet.PendingAmount -= transactionCreate.Amount;
            receiveWallet.PendingAmount += transactionCreate.Amount;

            _context.Wallets.Update(sendWallet);
            _context.Wallets.Update(receiveWallet);
            _context.BookingTransactions.Add(transaction);
            _context.WalletTransactions.Add(senderTransaction);

            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = "Giao dịch booking",
                Content = "Bạn đã nhận được một giao dịch booking với số tiền là " + transactionCreate.Amount + " VND. Mã giao dịch: " + transaction.BookingTransactionId,
                UserId = findUser.Id,
                CreatedAt = DateTime.Now,
                Status = (int)NotificationEnum.UnRead
            };
            _context.Notifications.Add(notification);
            _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification.NotificationId, notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = findUser.Email,
                Subject = "Xác nhận giao dịch",
                Body = "Bạn đã tạo giao dịch booking với số tiền là " + transactionCreate.Amount + " VND. Vui lòng xác nhận giao dịch tại đây: " + transactionCreate.RedirectUrl
            });
            return new StatusCodeResult(201);
        }
        public async Task<IActionResult> CreateDepositVnPayCourse(CourseTransactionCreate transactionCreate)
        {
            var findUser = _context.Users.FirstOrDefault(u => u.Id == transactionCreate.SenderId);
            if (findUser == null)
            {
                return new StatusCodeResult(404);
            }
            var senderWallet = _context.Wallets.Include(w => w.SenderCourseTransactionsNavigation.FirstOrDefault(w => w.SenderWalletId.Equals(transactionCreate.SenderId)));
            var receiverWallet = _context.Wallets.Include(w => w.ReceiverCourseTransactionsNavigation.FirstOrDefault(w => w.ReceiverWalletId.Equals(transactionCreate.ReceiverId)));
            if (senderWallet == null || receiverWallet == null)
            {
                return new StatusCodeResult(404);
            }

            CourseTransaction transaction = new CourseTransaction
            {
                CourseTransactionId = Guid.NewGuid(),
                SenderWalletId = (Guid)transactionCreate.SenderId,
                ReceiverWalletId = (Guid)transactionCreate.ReceiverId,
                CourseId = transactionCreate.CourseId,
                CreatedAt = DateTime.Now,
                Amount = transactionCreate.Amount,
                Status = (int)VNPayType.PENDING,
            };

            WalletTransaction senderTransaction = new WalletTransaction
            {
                WalletTransactionId = transaction.CourseTransactionId,
                SenderWalletId = (Guid)transactionCreate.SenderId,
                ReceiverWalletId = (Guid)transactionCreate.ReceiverId,
                Amount = transactionCreate.Amount,
                CreatedAt = DateTime.Now,
                Status = (int)VNPayType.PENDING,
                Note = "Giao dịch đặt khóa học"
            };
            var sendWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.SenderId);
            var receiveWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.ReceiverId);
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

            _context.Wallets.Update(sendWallet);
            _context.Wallets.Update(receiveWallet);

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
        public async Task<IActionResult> UpdateTransaction(Guid walletTransactionId, int choice, int updateStatus)
        {
            var wallet = await _context.WalletTransactions.FirstOrDefaultAsync(w => w.WalletTransactionId == walletTransactionId);
            var sender = await _context.Users.Include(c => c.WalletNavigation).FirstOrDefaultAsync(u => u.WalletNavigation.WalletId == wallet.SenderWalletId);
            var receiver = await _context.Users.Include(c => c.WalletNavigation).FirstOrDefaultAsync(u => u.WalletNavigation.WalletId == wallet.ReceiverWalletId);
            //only accept pending transaction
            if (wallet == null)
            {
                return new StatusCodeResult(404);
            }
            if (wallet.Status == (Int32)VNPayType.APPROVE || wallet.Status == (Int32)VNPayType.REJECT)
            {
                return new StatusCodeResult(409);
            }
            if (updateStatus == (Int32)VNPayType.APPROVE)
            {
                if (choice == (Int32)UpdateTransactionType.Booking)
                {
                    wallet.Status = (int)VNPayType.APPROVE;
                    var booking = _context.BookingTransactions.FirstOrDefault(b => b.BookingTransactionId == wallet.WalletTransactionId);
                    booking.Status = (int)VNPayType.APPROVE;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.Amount -= booking.Amount;
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.Now;
                    wallet.SenderWalletNavigation.PendingAmount -= booking.Amount;
                    wallet.SenderWalletNavigation.AvalaibleAmount -= booking.Amount;

                    wallet.ReceiverWalletNavigation.Amount += booking.Amount;
                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.Now;
                    wallet.ReceiverWalletNavigation.AvalaibleAmount += booking.Amount;
                    wallet.ReceiverWalletNavigation.PendingAmount -= booking.Amount;

                    _context.BookingTransactions.Update(booking);
                    _context.WalletTransactions.Update(wallet);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = sender.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch booking của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = receiver.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch booking của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    var notification1 = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch booking",
                        Content = "Giao dịch booking của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch booking",
                        Content = "Giao dịch booking của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification1.NotificationId, notification1);
                    _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification2.NotificationId, notification2);

                }
                else if (choice == (Int32)UpdateTransactionType.Course)
                {
                    wallet.Status = (int)VNPayType.APPROVE;
                    var course = _context.CourseTransactions.FirstOrDefault(b => b.CourseTransactionId == wallet.WalletTransactionId);
                    course.Status = (int)VNPayType.APPROVE;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.Amount -= course.Amount;
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.Now;
                    wallet.SenderWalletNavigation.PendingAmount -= course.Amount;
                    wallet.SenderWalletNavigation.AvalaibleAmount -= course.Amount;

                    wallet.ReceiverWalletNavigation.Amount += course.Amount;
                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.Now;
                    wallet.ReceiverWalletNavigation.AvalaibleAmount += course.Amount;
                    wallet.ReceiverWalletNavigation.PendingAmount -= course.Amount;

                    _context.CourseTransactions.Update(course);
                    _context.WalletTransactions.Update(wallet);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = sender.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch course của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = receiver.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch course của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    var notification1 = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch course",
                        Content = "Giao dịch course của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch course",
                        Content = "Giao dịch course của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification1.NotificationId, notification1);
                    _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification2.NotificationId, notification2);
                }
                else if (choice == (Int32)UpdateTransactionType.Wallet)
                {
                    wallet.Status = (int)VNPayType.APPROVE;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.Amount -= wallet.Amount;
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.Now;
                    wallet.SenderWalletNavigation.PendingAmount -= wallet.Amount;
                    wallet.SenderWalletNavigation.AvalaibleAmount -= wallet.Amount;

                    wallet.ReceiverWalletNavigation.Amount += wallet.Amount;
                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.Now;
                    wallet.ReceiverWalletNavigation.AvalaibleAmount += wallet.Amount;
                    wallet.ReceiverWalletNavigation.PendingAmount -= wallet.Amount;


                    _context.WalletTransactions.Update(wallet);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = sender.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch ví của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = receiver.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch ví của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    var notification1 = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch ví",
                        Content = "Giao dịch ví của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch ví",
                        Content = "Giao dịch ví của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification1.NotificationId, notification1);
                    _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification2.NotificationId, notification2);
                }
                else if (choice == (Int32)UpdateTransactionType.Unknown) { return new StatusCodeResult(406); }
            }
            else if (updateStatus == (Int32)VNPayType.REJECT)
            {
                if (choice == (Int32)UpdateTransactionType.Booking)
                {
                    wallet.Status = (int)VNPayType.REJECT;
                    var booking = _context.BookingTransactions.FirstOrDefault(b => b.BookingTransactionId == wallet.WalletTransactionId);
                    booking.Status = (int)VNPayType.REJECT;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.Now;
                    wallet.SenderWalletNavigation.PendingAmount -= booking.Amount;

                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.Now;
                    wallet.ReceiverWalletNavigation.PendingAmount -= booking.Amount;

                    _context.BookingTransactions.Update(booking);
                    _context.WalletTransactions.Update(wallet);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = sender.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch booking của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = receiver.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch booking của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    var notification1 = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch booking",
                        Content = "Giao dịch booking của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch booking",
                        Content = "Giao dịch booking của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification1.NotificationId, notification1);
                    _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification2.NotificationId, notification2);
                }
                else if (choice == (Int32)UpdateTransactionType.Course)
                {
                    wallet.Status = (int)VNPayType.REJECT;
                    var course = _context.CourseTransactions.FirstOrDefault(b => b.CourseTransactionId == wallet.WalletTransactionId);
                    course.Status = (int)VNPayType.REJECT;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.Now;
                    wallet.SenderWalletNavigation.PendingAmount -= course.Amount;

                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.Now;
                    wallet.ReceiverWalletNavigation.PendingAmount -= course.Amount;

                    _context.CourseTransactions.Update(course);
                    _context.WalletTransactions.Update(wallet);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = sender.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch course của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = receiver.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch course của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    var notification1 = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch course",
                        Content = "Giao dịch course của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch course",
                        Content = "Giao dịch course của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification1.NotificationId, notification1);
                    _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification2.NotificationId, notification2);
                }
                else if (choice == (Int32)UpdateTransactionType.Wallet)
                {
                    wallet.Status = (int)VNPayType.REJECT;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.Now;
                    wallet.SenderWalletNavigation.PendingAmount -= wallet.Amount;

                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.Now;
                    wallet.ReceiverWalletNavigation.PendingAmount -= wallet.Amount;

                    _context.WalletTransactions.Update(wallet);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = sender.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch ví của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = receiver.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch ví của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    var notification1 = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch ví",
                        Content = "Giao dịch ví của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch ví",
                        Content = "Giao dịch ví của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.Now,
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification1.NotificationId, notification1);
                    _firebaseRealtimeDatabaseService.SetAsync("Notifications/" + notification2.NotificationId, notification2);
                }
                else if (choice == (Int32)UpdateTransactionType.Unknown) { return new StatusCodeResult(406); }
            }
            else if (updateStatus == (Int32)VNPayType.PENDING)
            {
                return new StatusCodeResult(406);
            }

            await _context.SaveChangesAsync();
            return new StatusCodeResult(200);
        }
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionByAccountId(Guid id)
        {
            var findUser = _context.Users.FirstOrDefault(u => u.Id == id);
            if (findUser == null)
            {
                return new StatusCodeResult(405);
            }
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == id);
            var walletTransaction = await _context.WalletTransactions.Where(a => a.SenderWalletId == wallet.WalletId || a.ReceiverWalletId == wallet.WalletId).ToListAsync();
            if (walletTransaction == null)
            {
                return new StatusCodeResult(404);
            }
            return walletTransaction;
        }
        public async Task<ActionResult<PageResults<WalletTransaction>>> GetWalletTransactionByAccountIdPaging(Guid accountId, PagingRequest request)
        {
            var findUser = _context.Users.FirstOrDefault(u => u.Id == accountId);
            if (findUser == null)
            {
                return new StatusCodeResult(404);
            }
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == accountId);
            var walletTransactionsList = await _context.WalletTransactions
                .Where(a => a.SenderWalletId == wallet.WalletId || a.ReceiverWalletId == wallet.WalletId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            if (walletTransactionsList == null || !walletTransactionsList.Any())
            {
                return new StatusCodeResult(404);
            }

            var paginatedWalletTransactions = PagingHelper<WalletTransaction>.Paging(walletTransactionsList, request.Page, request.PageSize);
            if (paginatedWalletTransactions == null)
            {
                return new StatusCodeResult(400);
            }

            return paginatedWalletTransactions;
        }

        public async Task<ActionResult<List<BookingTransaction>>> GetAllBookingTransactions()
        {
            try
            {
                var bookingTransactions = await _context.BookingTransactions.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (bookingTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookingTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<BookingTransaction>>> GetAllBookingTransactionsPaging(PagingRequest request)
        {
            try
            {
                var bookingTransactionsList = await _context.BookingTransactions.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (bookingTransactionsList == null || !bookingTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedBookingTransactions = PagingHelper<BookingTransaction>.Paging(bookingTransactionsList, request.Page, request.PageSize);
                if (paginatedBookingTransactions == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedBookingTransactions;
            }
            catch (Exception ex)
            {
               throw new Exception(ex.ToString());
            }
        }

        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsBySenderId(Guid id)
        {
            try
            {
                var bookingTransactions = await _context.BookingTransactions.Where(c => c.SenderWalletId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (bookingTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookingTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<BookingTransaction>>> GetBookingTransactionsBySenderIdPaging(Guid id, PagingRequest request)
        {
            try
            {
                var bookingTransactionsList = await _context.BookingTransactions
                    .Where(c => c.SenderWalletId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (bookingTransactionsList == null || !bookingTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedBookingTransactions = PagingHelper<BookingTransaction>.Paging(bookingTransactionsList, request.Page, request.PageSize);
                if (paginatedBookingTransactions == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedBookingTransactions;
            }
            catch (Exception ex)
            {
               throw new Exception(ex.ToString());
            }
        }

        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsByReceiverId(Guid id)
        {
            try
            {
                var bookingTransactions = await _context.BookingTransactions.Where(c => c.ReceiverWalletId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (bookingTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookingTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<BookingTransaction>>> GetBookingTransactionsByReceiverIdPaging(Guid id, PagingRequest request)
        {
            try
            {
                var bookingTransactionsList = await _context.BookingTransactions
                    .Where(c => c.ReceiverWalletId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (bookingTransactionsList == null || !bookingTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedBookingTransactions = PagingHelper<BookingTransaction>.Paging(bookingTransactionsList, request.Page, request.PageSize);
                if (paginatedBookingTransactions == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedBookingTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<BookingTransaction>>> GetBookingTransactionsByBookingId(Guid id)
        {
            try
            {
                var bookingTransactions = await _context.BookingTransactions.Where(c => c.BookingId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (bookingTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookingTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<BookingTransaction>>> GetBookingTransactionsByBookingIdPaging(Guid id, PagingRequest request)
        {
            try
            {
                var bookingTransactionsList = await _context.BookingTransactions
                    .Where(c => c.BookingId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (bookingTransactionsList == null || !bookingTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedBookingTransactions = PagingHelper<BookingTransaction>.Paging(bookingTransactionsList, request.Page, request.PageSize);
                if (paginatedBookingTransactions == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedBookingTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<WalletTransaction>>> GetAllWalletTransactions()
        {
            try
            {
                var walletTransactions = await _context.WalletTransactions.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (walletTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return walletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<WalletTransaction>>> GetAllWalletTransactionsPaging(PagingRequest request)
        {
            try
            {
                var walletTransactionsList = await _context.WalletTransactions.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (walletTransactionsList == null || !walletTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedWalletTransactions = PagingHelper<WalletTransaction>.Paging(walletTransactionsList, request.Page, request.PageSize);
                if (paginatedWalletTransactions == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedWalletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsByWalletTransactionId(Guid id)
        {
            try
            {
                var walletTransactions = await _context.WalletTransactions.Where(c => c.WalletTransactionId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (walletTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return walletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<WalletTransaction>>> GetWalletTransactionsByWalletTransactionIdPaging(Guid id, PagingRequest request)
        {
            try
            {
                var walletTransactionsList = await _context.WalletTransactions
                    .Where(c => c.WalletTransactionId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (walletTransactionsList == null || !walletTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedWalletTransactions = PagingHelper<WalletTransaction>.Paging(walletTransactionsList, request.Page, request.PageSize);
                if (paginatedWalletTransactions == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedWalletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionByWalletId(Guid id)
        {
            try
            {
                var walletTransactions = await _context.WalletTransactions.Where(c => (c.SenderWalletId == id || c.ReceiverWalletId == id)).ToListAsync();
                if (walletTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return walletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<WalletTransaction>>> GetWalletTransactionByWalletIdPaging(Guid id, PagingRequest request)
        {
            try
            {
                var walletTransactionsList = await _context.WalletTransactions
                    .Where(c => c.SenderWalletId == id || c.ReceiverWalletId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (walletTransactionsList == null || !walletTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedWalletTransactions = PagingHelper<WalletTransaction>.Paging(walletTransactionsList, request.Page, request.PageSize);
                if (paginatedWalletTransactions == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedWalletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsBySenderId(Guid id)
        {
            try
            {
                var walletTransactions = await _context.WalletTransactions.Where(c => c.SenderWalletId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (walletTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return walletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<WalletTransaction>>> GetWalletTransactionsBySenderIdPaging(Guid id, PagingRequest request)
        {
            try
            {
                var walletTransactionsList = await _context.WalletTransactions
                    .Where(c => c.SenderWalletId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (walletTransactionsList == null || !walletTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedWalletTransactions = PagingHelper<WalletTransaction>.Paging(walletTransactionsList, request.Page, request.PageSize);
                if (paginatedWalletTransactions == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedWalletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<WalletTransaction>>> GetWalletTransactionsByReceiverId(Guid id)
        {
            try
            {
                var walletTransactions = await _context.WalletTransactions.Where(c => c.ReceiverWalletId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (walletTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return walletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<WalletTransaction>>> GetWalletTransactionsByReceiverIdPaging(Guid id, PagingRequest request)
        {
            try
            {
                var walletTransactionsList = await _context.WalletTransactions
                    .Where(c => c.ReceiverWalletId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (walletTransactionsList == null || !walletTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedWalletTransactions = PagingHelper<WalletTransaction>.Paging(walletTransactionsList, request.Page, request.PageSize);
                if (paginatedWalletTransactions == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedWalletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<CourseTransaction>>> GetAllCourseTransactions()
        {
            try
            {
                var courseTransactions = await _context.CourseTransactions.ToListAsync();
                if (courseTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<CourseTransaction>>> GetAllCourseTransactionsPaging(PagingRequest request)
        {
            try
            {
                var courseTransactionsList = await _context.CourseTransactions.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (courseTransactionsList == null || !courseTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedCourseTransactions = PagingHelper<CourseTransaction>.Paging(courseTransactionsList, request.Page, request.PageSize);
                if (paginatedCourseTransactions == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedCourseTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<CourseTransaction>> GetCourseTransaction(Guid id)
        {
            try
            {
                var courseTransaction = await _context.CourseTransactions.FirstOrDefaultAsync(c => c.CourseTransactionId == id);
                if (courseTransaction == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseTransaction;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<BookingTransaction>> GetBookingTransaction(Guid id)
        {
            try
            {
                var bookingTransaction = await _context.BookingTransactions.FirstOrDefaultAsync(c => c.BookingTransactionId == id);
                if (bookingTransaction == null)
                {
                    return new StatusCodeResult(404);
                }
                return bookingTransaction;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<WalletTransaction>> GetWalletTransaction(Guid id)
        {
            try
            {
                var walletTransaction = await _context.WalletTransactions.FirstOrDefaultAsync(c => c.WalletTransactionId == id);
                if (walletTransaction == null)
                {
                    return new StatusCodeResult(404);
                }
                return walletTransaction;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsBySenderId(Guid id)
        {
            try
            {
                var courseTransactions = await _context.CourseTransactions.Where(c => c.SenderWalletId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (courseTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<CourseTransaction>>> GetCourseTransactionBySenderIdPaging(Guid id, PagingRequest request)
        {
            try
            {
                var courseTransactionsList = await _context.CourseTransactions
                    .Where(c => c.SenderWalletId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (courseTransactionsList == null || !courseTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedCourseTransactions = PagingHelper<CourseTransaction>.Paging(courseTransactionsList, request.Page, request.PageSize);
                if (paginatedCourseTransactions == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedCourseTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByReceiverId(Guid id)
        {
            try
            {
                var courseTransactions = await _context.CourseTransactions.Where(c => c.ReceiverWalletId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (courseTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<CourseTransaction>>> GetCourseTransactionsByReceiverIdPaging(Guid id, PagingRequest request)
        {
            try
            {
                var courseTransactionsList = await _context.CourseTransactions
                    .Where(c => c.ReceiverWalletId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (courseTransactionsList == null || !courseTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedCourseTransactions = PagingHelper<CourseTransaction>.Paging(courseTransactionsList, request.Page, request.PageSize);
                if (paginatedCourseTransactions == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedCourseTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<CourseTransaction>>> GetCourseTransactionsByCourseId(Guid id)
        {
            try
            {
                var courseTransactions = await _context.CourseTransactions.Where(c => c.CourseId == id).OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (courseTransactions == null)
                {
                    return new StatusCodeResult(404);
                }
                return courseTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<CourseTransaction>>> GetCourseTransactionsByCourseIdPaging(Guid id, PagingRequest request)
        {
            try
            {
                var courseTransactionsList = await _context.CourseTransactions
                    .Where(c => c.CourseId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (courseTransactionsList == null || !courseTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedCourseTransactions = PagingHelper<CourseTransaction>.Paging(courseTransactionsList, request.Page, request.PageSize);
                if (paginatedCourseTransactions == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedCourseTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
