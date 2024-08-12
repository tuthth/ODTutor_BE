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
using FirebaseAdmin.Messaging;
using System.Net;
using Models.Migrations;
using Org.BouncyCastle.Tls;

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
                var senderWallet = _context.Wallets.Include(w => w.SenderWalletTransactionsNavigation.FirstOrDefault(w => w.SenderWalletId.Equals(transactionCreate.SenderId)));
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
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    Amount = transactionCreate.Amount,
                    Status = (int)VNPayType.PENDING,
                    Note = "Nạp tiền vào tài khoản"
                };
                var sendWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.SenderId);
                sendWallet.PendingAmount -= transactionCreate.Amount;
                sendWallet.AvalaibleAmount -= transactionCreate.Amount;
                _context.Wallets.Update(sendWallet);
                var receiveWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.ReceiverId);
                receiveWallet.PendingAmount += transactionCreate.Amount;

                _context.Wallets.Update(receiveWallet);
                _context.WalletTransactions.Add(transaction);

                var notification1x = new NotificationDTO
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "Nạp tiền vào tài khoản",
                    Content = "Bạn đã nhận được một giao dịch nạp tiền vào tài khoản với số tiền là " + transactionCreate.Amount + " VND. Mã giao dịch: " + transaction.WalletTransactionId,
                    UserId = findUser.Id,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    Status = (int)NotificationEnum.UnRead
                };
                Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification1x);
                _context.Notifications.Add(notification1);
                _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification1x.UserId}/{notification1x.NotificationId}", notification1x);
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
                vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(_httpContextAccessor));
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", transaction.WalletTransactionId.ToString());
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", DateTime.UtcNow.AddHours(7).Ticks.ToString());

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
                var receiverWallet = _context.Wallets.Include(w => w.ReceiverWalletTransactionsNavigation.FirstOrDefault(w => w.ReceiverWalletId.Equals(transactionCreate.ReceiverId)));
                if (senderWallet == null)
                {
                    return new StatusCodeResult(404);
                }
                WalletTransaction transaction = new WalletTransaction
                {
                    WalletTransactionId = Guid.NewGuid(),
                    SenderWalletId = (Guid)transactionCreate.SenderId,
                    ReceiverWalletId = (Guid)transactionCreate.ReceiverId, // This should be the admin's wallet ID
                    CreatedAt = DateTime.UtcNow.AddHours(7),
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
                sendWallet.AvalaibleAmount -= transactionCreate.Amount;
                _context.Wallets.Update(sendWallet);
                var receiveWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.ReceiverId);
                receiveWallet.PendingAmount += transactionCreate.Amount; // Add the amount to the admin's wallet
                _context.Wallets.Update(receiveWallet);
                _context.WalletTransactions.Add(transaction);

                var notification1x = new NotificationDTO
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "Rút tiền từ tài khoản",
                    Content = "Bạn đã tạo một giao dịch rút tiền từ tài khoản với số tiền là " + transactionCreate.Amount + " VND. Mã giao dịch: " + transaction.WalletTransactionId,
                    UserId = findUser.Id,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    Status = (int)NotificationEnum.UnRead
                };
                Models.Entities.Notification noti = _mapper.Map<Models.Entities.Notification>(notification1x);
                _context.Notifications.Add(noti);
                _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification1x.UserId}/{notification1x.NotificationId}", notification1x);
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
                vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(_httpContextAccessor));
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", transaction.WalletTransactionId.ToString());
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", DateTime.UtcNow.AddHours(7).Ticks.ToString());

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
            var notificationError = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Giao dịch",
                Content = "Giao dịch không hợp lệ, vui lòng kiểm tra lại thông tin.",
                UserId = findUser.Id,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (int)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification = _mapper.Map<Models.Entities.Notification>(notificationError);
            _context.Notifications.Add(notification);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notificationError.UserId}/{notificationError.NotificationId}", notificationError);
            await _context.SaveChangesAsync();
            return new StatusCodeResult(500);
        }

        public async Task<IActionResult> UpgradeAccount(WalletTransactionCreate transactionCreate)
        {
            if (transactionCreate.Choice != (Int32)VNPayTransactionType.Upgrade && transactionCreate.Choice != (Int32)VNPayTransactionType.StudentSubscription)
            {
                return new StatusCodeResult(406);
            }
            var findUser = _context.Users.Include(u => u.WalletNavigation).FirstOrDefault(u => u.WalletNavigation.WalletId == transactionCreate.SenderId);
            if (findUser == null)
            {
                return new StatusCodeResult(404);
            }
            if (transactionCreate.Choice == (Int32)VNPayTransactionType.Upgrade)
            {
                if (findUser.IsPremium == true)
                {
                    return new StatusCodeResult(409);
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
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    Amount = transactionCreate.Amount,
                    Status = (int)VNPayType.APPROVE,
                    Note = "Nâng cấp tài khoản giáo viên"
                };
                var receiveWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.ReceiverId);
                var sendWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.SenderId);
                if (sendWallet.Amount < transactionCreate.Amount)
                {
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = findUser.Email,
                        Subject = "Nâng cấp tài khoản",
                        Body = "Giao dịch nâng cấp không hợp lệ, vui lòng kiểm tra lại số dư tài khoản."
                    });
                    var notificationError = new NotificationDTO
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch nâng cấp tài khoản",
                        Content = "Giao dịch nâng cấp không hợp lệ, vui lòng kiểm tra lại số dư tài khoản.",
                        UserId = findUser.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notificationError);
                    _context.Notifications.Add(notification1);
                    _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notificationError.UserId}/{notificationError.NotificationId}", notificationError);
                    return new StatusCodeResult(409);
                }
                receiveWallet.Amount += transactionCreate.Amount;
                sendWallet.Amount -= transactionCreate.Amount;

                _context.WalletTransactions.Add(transaction);
                _context.Wallets.Update(receiveWallet);
                _context.Wallets.Update(sendWallet);
                findUser.IsPremium = true;
                _context.Users.Update(findUser);

                var notification = new Models.Entities.Notification
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "Nâng cấp tài khoản",
                    Content = "Tài khoản",
                    UserId = findUser.Id,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    Status = (int)NotificationEnum.UnRead
                };
                _context.Notifications.Add(notification);
                _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                await _context.SaveChangesAsync();
                await _appExtension.SendMail(new MailContent()
                {
                    To = findUser.Email,
                    Subject = "Nâng cấp tài khoản",
                    Body = "Tài khoản của bạn đã được nâng cấp thành Premium. Hãy truy cập hệ thống để trải nghiệm đầy đủ tính năng. \nMã giao dịch: " + transaction.WalletTransactionId
                });
                return new JsonResult(new
                {
                    WalletTransactionId = transaction.WalletTransactionId,
                    Status = transaction.Status
                });

            }
            else if (transactionCreate.Choice == (Int32)VNPayTransactionType.StudentSubscription)
            {
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
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    Amount = transactionCreate.Amount,
                    Status = (int)VNPayType.APPROVE,
                    Note = "Nâng cấp tài khoản học viên"
                };
                var receiveWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.ReceiverId);
                var sendWallet = _context.Wallets.FirstOrDefault(w => w.WalletId == transactionCreate.SenderId);
                if (sendWallet.Amount < transactionCreate.Amount)
                {
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = findUser.Email,
                        Subject = "Nâng cấp yêu cầu giáo viên của tài khoản",
                        Body = "Giao dịch nâng cấp không hợp lệ, vui lòng kiểm tra lại số dư tài khoản."
                    });
                    var notificationError = new NotificationDTO
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch nâng cấp yêu cầu giáo viên của tài khoản",
                        Content = "Giao dịch nâng cấp không hợp lệ, vui lòng kiểm tra lại số dư tài khoản.",
                        UserId = findUser.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notificationError);
                    _context.Notifications.Add(notification1);
                    _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notificationError.UserId}/{notificationError.NotificationId}", notificationError);
                    return new StatusCodeResult(409);
                }
                receiveWallet.Amount += transactionCreate.Amount;
                sendWallet.Amount -= transactionCreate.Amount;

                _context.WalletTransactions.Add(transaction);
                _context.Wallets.Update(receiveWallet);
                _context.Wallets.Update(sendWallet);
                findUser.HasBoughtSubscription = true;
                _context.Users.Update(findUser);

                var notification = new Models.Entities.Notification
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "Nâng cấp yêu cầu giáo viên của tài khoản",
                    Content = "Tài khoản",
                    UserId = findUser.Id,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    Status = (int)NotificationEnum.UnRead
                };
                _context.Notifications.Add(notification);
                _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                await _context.SaveChangesAsync();
                await _appExtension.SendMail(new MailContent()
                {
                    To = findUser.Email,
                    Subject = "Nâng cấp yêu cầu giáo viên của tài khoản",
                    Body = "Tài khoản của bạn đã được nâng cấp thành Premium. Hãy truy cập hệ thống để trải nghiệm đầy đủ tính năng. \nMã giao dịch: " + transaction.WalletTransactionId
                });
                return new JsonResult(new
                {
                    WalletTransactionId = transaction.WalletTransactionId,
                    Status = transaction.Status
                });
            }
            return new StatusCodeResult(500);
        }


        public async Task<IActionResult> HasBoughtTutorExperiencePackage(WalletTransactionCreate request)
        {
            try
            {
                var receiverId = new Guid("d71b17cc-7997-4b23-2adc-08dc93ff561f");
                var findUser = _context.Users.Include(u => u.WalletNavigation).FirstOrDefault(u => u.Id == request.SenderId);
                var receiver = _context.Users.Include(u => u.WalletNavigation)
                        .FirstOrDefault(u => u.Id == receiverId);
                if (findUser == null || receiver == null)
                {
                    return new StatusCodeResult(404);
                }
                // Find Wallet of FindUser
                var walletUser = _context.Wallets.FirstOrDefault(w => w.WalletId == findUser.WalletNavigation.WalletId);
                var walletReceiver = _context.Wallets.FirstOrDefault(w => w.WalletId == receiver.WalletNavigation.WalletId);
                if (request.Choice == (Int32)VNPayTransactionType.TutorExperienceSubscription)
                {
                    var senderWallet = _context.Wallets.Include(w => w.SenderWalletTransactionsNavigation.FirstOrDefault(w => w.SenderWalletId.Equals(walletUser.WalletId)));
                    var receiverWallet = _context.Wallets.Include(w => w.ReceiverWalletTransactionsNavigation.FirstOrDefault(w => w.ReceiverWalletId.Equals(walletReceiver.WalletId)));
                    if (senderWallet == null || receiverWallet == null)
                    {
                        return new StatusCodeResult(404);
                    }
                    WalletTransaction transaction = new WalletTransaction
                    {
                        WalletTransactionId = Guid.NewGuid(),
                        SenderWalletId = (Guid)walletUser.WalletId,
                        ReceiverWalletId = (Guid)walletReceiver.WalletId,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Amount = request.Amount,
                        Status = (int)VNPayType.APPROVE,
                        Note = "Nâng cấp tài khoản gói thành viên trải nghiệm gia sư"
                    };
                    if (walletUser.Amount < request.Amount)
                    {
                        await _appExtension.SendMail(new MailContent()
                        {
                            To = findUser.Email,
                            Subject = "Nâng cấp tài khoản",
                            Body = "Giao dịch nâng cấp không hợp lệ, vui lòng kiểm tra lại số dư tài khoản."
                        });
                        var notificationError = new NotificationDTO
                        {
                            NotificationId = Guid.NewGuid(),
                            Title = "Giao dịch nâng cấp tài khoản",
                            Content = "Giao dịch nâng cấp không hợp lệ, vui lòng kiểm tra lại số dư tài khoản.",
                            UserId = findUser.Id,
                            CreatedAt = DateTime.UtcNow.AddHours(7),
                            Status = (int)NotificationEnum.UnRead
                        };
                        Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notificationError);
                        _context.Notifications.Add(notification1);
                        _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notificationError.UserId}/{notificationError.NotificationId}", notificationError);
                        return new StatusCodeResult(409);
                    }

                    walletUser.Amount -= request.Amount;
                    walletReceiver.Amount += request.Amount;

                    _context.WalletTransactions.Add(transaction);
                    _context.Wallets.Update(walletUser);
                    _context.Wallets.Update(walletReceiver);
                    Tutor tutor = _context.Tutors.FirstOrDefault(t => t.UserId == findUser.Id);
                    tutor.HasBoughtExperiencedPackage = true;
                    tutor.HasBoughtSubscription = true;
                    // Format startDate
                    DateTime startTimeDate = DateTime.UtcNow.AddHours(7);
                    
                    // Định dạng ngày và giờ 
                    string formatStartTimeDate  = startTimeDate.ToString("dd-MM-yyyy") + " 00:00:00";
                    tutor.SubcriptionStartDate = DateTime.ParseExact(formatStartTimeDate, "dd-MM-yyyy HH:mm:ss", null);

                    // Format endDate 
                    DateTime endTimeDate = DateTime.UtcNow.AddHours(7).AddDays(30);

                    // Định dạng ngày và giờ
                    string formatEndTimeDate = endTimeDate.ToString("dd-MM-yyyy") + " 00:00:00";
                    tutor.SubcriptionEndDate = DateTime.ParseExact(formatEndTimeDate, "dd-MM-yyyy HH:mm:ss", null);
                    tutor.SubcriptionType = (Int32)TutorPackageEnum.Experience;
                    _context.Update(tutor);
                    var notification = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Nâng cấp tài khoản",
                        Content = "Tài khoản",
                        UserId = findUser.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                    await _context.SaveChangesAsync();
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = findUser.Email,
                        Subject = "Nâng cấp tài khoản",
                        Body = " Cảm ơn bạn đã mua gói trải nghiệm bên chúng tôi cảm ơn bạn rất nhiều. Hãy truy cập hệ thống để trải nghiệm đầy đủ tính năng. \nMã giao dịch: " + transaction.WalletTransactionId
                    });
                    return new JsonResult(new
                    {
                        WalletTransactionId = transaction.WalletTransactionId,
                        Status = transaction.Status
                    });

                }

                return new StatusCodeResult(500);

            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> HasBoughtTutorPackage(WalletTransactionCreate request)
        {
            try
            {
                var receiverId = new Guid("d71b17cc-7997-4b23-2adc-08dc93ff561f");
                var findUser = _context.Users.Include(u => u.WalletNavigation).FirstOrDefault(u => u.Id == request.SenderId);
                var receiver = _context.Users.Include(u => u.WalletNavigation)
                        .FirstOrDefault(u => u.Id == receiverId);
                if (findUser == null || receiver == null)
                {
                    return new StatusCodeResult(404);
                }
                // Find Wallet of FindUser
                var walletUser = _context.Wallets.FirstOrDefault(w => w.WalletId == findUser.WalletNavigation.WalletId);
                var walletReceiver = _context.Wallets.FirstOrDefault(w => w.WalletId == receiver.WalletNavigation.WalletId);
                if (request.Choice == (Int32)VNPayTransactionType.TutorSubscription)
                {
                    var senderWallet = _context.Wallets.Include(w => w.SenderWalletTransactionsNavigation.FirstOrDefault(w => w.SenderWalletId.Equals(walletUser.WalletId)));
                    var receiverWallet = _context.Wallets.Include(w => w.ReceiverWalletTransactionsNavigation.FirstOrDefault(w => w.ReceiverWalletId.Equals(walletReceiver.WalletId)));
                    if (senderWallet == null || receiverWallet == null)
                    {
                        return new StatusCodeResult(404);
                    }
                    WalletTransaction transaction = new WalletTransaction
                    {
                        WalletTransactionId = Guid.NewGuid(),
                        SenderWalletId = (Guid)walletUser.WalletId,
                        ReceiverWalletId = (Guid)walletReceiver.WalletId,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Amount = request.Amount,
                        Status = (int)VNPayType.APPROVE,
                        Note = "Nâng cấp tài khoản thành viên gia sư"
                    };
                    if (walletUser.Amount < request.Amount)
                    {
                        await _appExtension.SendMail(new MailContent()
                        {
                            To = findUser.Email,
                            Subject = "Nâng cấp tài khoản",
                            Body = "Giao dịch nâng cấp không hợp lệ, vui lòng kiểm tra lại số dư tài khoản."
                        });
                        var notificationError = new NotificationDTO
                        {
                            NotificationId = Guid.NewGuid(),
                            Title = "Giao dịch nâng cấp tài khoản",
                            Content = "Giao dịch nâng cấp không hợp lệ, vui lòng kiểm tra lại số dư tài khoản.",
                            UserId = findUser.Id,
                            CreatedAt = DateTime.UtcNow.AddHours(7),
                            Status = (int)NotificationEnum.UnRead
                        };
                        Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notificationError);
                        _context.Notifications.Add(notification1);
                        _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notificationError.UserId}/{notificationError.NotificationId}", notificationError);
                        return new StatusCodeResult(409);
                    }
                    walletUser.Amount -= request.Amount;
                    walletReceiver.Amount += request.Amount;
                    _context.WalletTransactions.Add(transaction);
                    _context.Wallets.Update(walletUser);
                    _context.Wallets.Update(walletReceiver);
                    Tutor tutor = _context.Tutors.FirstOrDefault(t => t.UserId == findUser.Id);
                    tutor.HasBoughtSubscription = true;
                    // Format startDate
                    DateTime startTimeDate = DateTime.UtcNow.AddHours(7).Date;
                    // Định dạng ngày và giờ
                    string formatStartTimeDate = startTimeDate.ToString("dd-MM-yyyy") + " 00:00:00";
                    tutor.SubcriptionStartDate = DateTime.ParseExact(formatStartTimeDate, "dd-MM-yyyy HH:mm:ss", null);
                    // Format endDate
                    DateTime endTimeDate = DateTime.UtcNow.AddHours(7).AddDays(30).Date;
                    // Định dạng ngày và giờ
                    string formatEndTimeDate = endTimeDate.ToString("dd-MM-yyyy") + " 00:00:00";
                    tutor.SubcriptionEndDate = DateTime.ParseExact(formatEndTimeDate, "dd-MM-yyyy HH:mm:ss", null);
                    tutor.SubcriptionType = (Int32)TutorPackageEnum.Premium;
                    _context.Update(tutor);
                    var notification = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Nâng cấp tài khoản",
                        Content = "Tài khoản",
                        UserId = findUser.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
                    await _context.SaveChangesAsync();
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = findUser.Email,
                        Subject = "Nâng cấp tài khoản",
                        Body = " Cảm ơn bạn đã mua gói thành viên bên chúng tôi cảm ơn bạn rất nhiều. Hãy truy cập hệ thống để trải nghiệm đầy đủ tính năng. \nMã giao dịch: " + transaction.WalletTransactionId
                    });
                    return new JsonResult(new
                    {
                        WalletTransactionId = transaction.WalletTransactionId,
                        Status = transaction.Status
                    });
                }
                return new StatusCodeResult(500);

            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }
        // Update Tutor Back Normal Tutor When Experience Subscription End
        public async Task<IActionResult> UpdateTutorBackNormalTutor(Guid tutorId)
        {
            try
            {
                var tutor = _context.Tutors
                    .Include(t => t.UserNavigation)
                    .FirstOrDefault(t => t.TutorId == tutorId);
                if (tutor == null)
                {
                    return new StatusCodeResult(404);
                }
                tutor.HasBoughtSubscription = false;
                tutor.SubcriptionStartDate = null;
                tutor.SubcriptionEndDate = null;
                tutor.SubcriptionType = (Int32)TutorPackageEnum.Standard;
                _context.Update(tutor);

                // Tạo thông báo cho tutor khi hết hạn 
                var notification = new NotificationDTO
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "Hết hạn gói trải nghiệm",
                    Content = "Gói trải nghiệm của bạn đã hết hạn. Hãy truy cập hệ thống để nâng cấp gói trải nghiệm.",
                    UserId = tutor.UserId,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    Status = (int)NotificationEnum.UnRead
                };
                Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
                _context.Notifications.Add(notification1);
                _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);

                // Send mail to tutor
                await _appExtension.SendMail(new MailContent()
                {
                    To = tutor.UserNavigation.Email,
                    Subject = "Hết hạn gói trải nghiệm",
                    Body = "Gói trải nghiệm của bạn đã hết hạn. Hãy truy cập hệ thống để nâng cấp gói trải nghiệm."
                });
                await _context.SaveChangesAsync();
                return new StatusCodeResult(200);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }
        public async Task<IActionResult> CreateDepositVnPayBooking(BookingTransactionCreate transactionCreate)
        {
            var user = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var findUser = _context.Users.Include(u => u.WalletNavigation).FirstOrDefault(u => u.WalletNavigation.WalletId == transactionCreate.SenderId);
            if (findUser == null)
            {
                return new StatusCodeResult(404);
            }
            var booking = _context.Bookings
                .Include(b => b.TutorNavigation)
                .FirstOrDefault(b => b.BookingId == transactionCreate.BookingId);
            if (booking == null)
            {
                return new StatusCodeResult(404);
            }
            if (booking.Status != (int)BookingEnum.WaitingPayment)
            {
                return new StatusCodeResult(406);
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
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Amount = transactionCreate.Amount,
                Status = (int)VNPayType.PENDING,
            };

            WalletTransaction senderTransaction = new WalletTransaction
            {
                WalletTransactionId = transaction.BookingTransactionId,
                SenderWalletId = (Guid)transactionCreate.SenderId,
                ReceiverWalletId = (Guid)transactionCreate.ReceiverId,
                Amount = transactionCreate.Amount,
                CreatedAt = DateTime.UtcNow.AddHours(7),
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
                var notificationError = new NotificationDTO
                {
                    NotificationId = Guid.NewGuid(),
                    Title = "Giao dịch booking",
                    Content = "Giao dịch booking không hợp lệ, vui lòng kiểm tra lại thông tin.",
                    UserId = findUser.Id,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    Status = (int)NotificationEnum.UnRead
                };
                Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notificationError);
                _context.Notifications.Add(notification1);
                _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notificationError.UserId}/{notificationError.NotificationId}", notificationError);
                return new StatusCodeResult(409);
            }
            sendWallet.PendingAmount -= transactionCreate.Amount;
            sendWallet.AvalaibleAmount -= transactionCreate.Amount;
            sendWallet.Amount -= transactionCreate.Amount;
            receiveWallet.PendingAmount += transactionCreate.Amount;

            _context.Wallets.Update(sendWallet);
            _context.Wallets.Update(receiveWallet);
            _context.BookingTransactions.Add(transaction);
            _context.WalletTransactions.Add(senderTransaction);
            // Changre status slot 
            TimeSpan bookingTime = new TimeSpan(booking.StudyTime.Value.Hour, booking.StudyTime.Value.Minute, 0);
            // Find the tutor available slot
            DateTime bookingDate = booking.StudyTime.Value.Date;
            var tutorDateAvailables = _context.TutorDateAvailables
                .Where(x => x.TutorID == booking.TutorId && x.Date.Date == bookingDate)
                .Select(x => x.TutorDateAvailableID)
                .ToList();
            if (tutorDateAvailables == null)
            {
                return new StatusCodeResult(452);
            }
            var tutorSlotAvailables = _context.TutorSlotAvailables
                .Where(x => tutorDateAvailables.Contains(x.TutorDateAvailable.TutorDateAvailableID) && x.StartTime == bookingTime)
                .FirstOrDefault();
            if (tutorSlotAvailables == null)
            {
                return new StatusCodeResult(453);
            }
            if (tutorSlotAvailables.IsBooked == true)
            {
                return new StatusCodeResult(454);
            }
            tutorSlotAvailables.IsBooked = true;
            tutorSlotAvailables.Status = (Int32)TutorSlotAvailabilityEnum.NotAvailable;
            booking.Status = (Int32)BookingEnum.Success;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Giao dịch booking",
                Content = "Bạn đã nhận được một giao dịch booking với số tiền là " + transactionCreate.Amount + " VND. Mã giao dịch: " + transaction.BookingTransactionId,
                UserId = findUser.Id,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (int)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification2 = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Notifications.Add(notification2);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = findUser.Email,
                Subject = "Xác nhận giao dịch",
                Body = "Bạn đã tạo giao dịch booking với số tiền là " + transactionCreate.Amount + " VND. Vui lòng xác nhận giao dịch tại đây: " + transactionCreate.RedirectUrl
            });
            return new JsonResult(new
            {
                BookingTransactionId = transaction.BookingTransactionId,
                Status = transaction.Status
            });
        }
        public async Task<IActionResult> CreateDepositVnPayCourse(CourseTransactionCreate transactionCreate)
        {
            var findUser = _context.Users.Include(u => u.WalletNavigation).FirstOrDefault(u => u.WalletNavigation.WalletId == transactionCreate.SenderId);
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

            Models.Entities.CourseTransaction transaction = new Models.Entities.CourseTransaction
            {
                CourseTransactionId = Guid.NewGuid(),
                SenderWalletId = (Guid)transactionCreate.SenderId,
                ReceiverWalletId = (Guid)transactionCreate.ReceiverId,
                CourseId = transactionCreate.CourseId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Amount = transactionCreate.Amount,
                Status = (int)VNPayType.PENDING,
            };

            WalletTransaction senderTransaction = new WalletTransaction
            {
                WalletTransactionId = transaction.CourseTransactionId,
                SenderWalletId = (Guid)transactionCreate.SenderId,
                ReceiverWalletId = (Guid)transactionCreate.ReceiverId,
                Amount = transactionCreate.Amount,
                CreatedAt = DateTime.UtcNow.AddHours(7),
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

            var notification = new NotificationDTO
            {
                NotificationId = Guid.NewGuid(),
                Title = "Giao dịch course",
                Content = "Bạn đã nhận được một giao dịch course với số tiền là " + transactionCreate.Amount + " VND. Mã giao dịch: " + transaction.CourseTransactionId,
                UserId = findUser.Id,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (int)NotificationEnum.UnRead
            };
            Models.Entities.Notification notification1 = _mapper.Map<Models.Entities.Notification>(notification);
            _context.Notifications.Add(notification1);
            _firebaseRealtimeDatabaseService.SetAsync<NotificationDTO>($"notifications/{notification.UserId}/{notification.NotificationId}", notification);

            _context.CourseTransactions.Add(transaction);
            _context.WalletTransactions.Add(senderTransaction);
            await _context.SaveChangesAsync();
            await _appExtension.SendMail(new MailContent()
            {
                To = findUser.Email,
                Subject = "Xác nhận giao dịch",
                Body = "Bạn đã tạo giao dịch course với số tiền là " + transactionCreate.Amount + " VND. Vui lòng xác nhận giao dịch tại đây: " + transactionCreate.RedirectUrl
            });
            return new JsonResult(new
            {
                CourseTransactionId = transaction.CourseTransactionId,
                Status = transaction.Status
            });
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
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.SenderWalletNavigation.PendingAmount += booking.Amount;

                    wallet.ReceiverWalletNavigation.Amount += booking.Amount;
                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.ReceiverWalletNavigation.AvalaibleAmount += booking.Amount;
                    wallet.ReceiverWalletNavigation.PendingAmount -= booking.Amount;
                    var book = _context.Bookings
                        .Include(b => b.TutorNavigation)
                        .FirstOrDefault(b => b.BookingId == booking.BookingId);
                    book.Status = (int)BookingEnum.Cancelled;
                    _context.BookingTransactions.Update(booking);
                    _context.WalletTransactions.Update(wallet);
                    _context.Bookings.Update(book);

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
                    var notification1 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch booking",
                        Content = "Giao dịch booking của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch booking",
                        Content = "Giao dịch booking của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification2.UserId}/{notification1.NotificationId}", notification2);
                }
                else if (choice == (Int32)UpdateTransactionType.Course)
                {
                    wallet.Status = (int)VNPayType.APPROVE;
                    var course = _context.CourseTransactions.FirstOrDefault(b => b.CourseTransactionId == wallet.WalletTransactionId);
                    course.Status = (int)VNPayType.APPROVE;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.Amount -= course.Amount;
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.SenderWalletNavigation.PendingAmount += course.Amount;
                    wallet.SenderWalletNavigation.AvalaibleAmount -= course.Amount;

                    wallet.ReceiverWalletNavigation.Amount += course.Amount;
                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
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
                    var notification1 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch course",
                        Content = "Giao dịch course của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch course",
                        Content = "Giao dịch course của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);

                    var courses = _context.Courses.Include(c => c.TutorNavigation).FirstOrDefault(c => c.CourseId == course.CourseId);
                    var student = _context.Students.FirstOrDefault(s => s.UserId == sender.Id);
                    var studentCourse = new StudentCourse
                    {
                        StudentCourseId = Guid.NewGuid(),
                        CourseId = course.CourseId,
                        StudentId = student.StudentId,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)CourseEnum.Active,
                        GoogleMeetUrl = ""
                    };
                    _context.StudentCourses.Add(studentCourse);
                }
                else if (choice == (Int32)UpdateTransactionType.Wallet)
                {
                    wallet.Status = (int)VNPayType.APPROVE;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.Amount -= wallet.Amount;
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.SenderWalletNavigation.PendingAmount += wallet.Amount;
                    wallet.SenderWalletNavigation.AvalaibleAmount -= wallet.Amount;

                    wallet.ReceiverWalletNavigation.Amount += wallet.Amount;
                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
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
                    var notification1 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch ví",
                        Content = "Giao dịch ví của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch ví",
                        Content = "Giao dịch ví của bạn đã được xác nhận. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);

                    //student course: chua ro query
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
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.SenderWalletNavigation.PendingAmount += booking.Amount;
                    wallet.SenderWalletNavigation.AvalaibleAmount += booking.Amount;
                    wallet.SenderWalletNavigation.Amount += booking.Amount;

                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
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
                    var notification1 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch booking",
                        Content = "Giao dịch booking của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch booking",
                        Content = "Giao dịch booking của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);
                    var book = _context.Bookings.FirstOrDefault(b => b.BookingId == booking.BookingId);
                    book.Status = (int)BookingEnum.Cancelled;

                    _context.Bookings.Update(book);
                }
                else if (choice == (Int32)UpdateTransactionType.Course)
                {
                    wallet.Status = (int)VNPayType.REJECT;
                    var course = _context.CourseTransactions.FirstOrDefault(b => b.CourseTransactionId == wallet.WalletTransactionId);
                    course.Status = (int)VNPayType.REJECT;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.SenderWalletNavigation.PendingAmount += course.Amount;
                    wallet.SenderWalletNavigation.AvalaibleAmount += course.Amount;
                    wallet.SenderWalletNavigation.Amount += course.Amount;

                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
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
                    var notification1 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch course",
                        Content = "Giao dịch course của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch course",
                        Content = "Giao dịch course của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);
                }
                else if (choice == (Int32)UpdateTransactionType.Wallet)
                {
                    wallet.Status = (int)VNPayType.REJECT;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.SenderWalletNavigation.PendingAmount += wallet.Amount;
                    wallet.SenderWalletNavigation.AvalaibleAmount += wallet.Amount;
                    wallet.SenderWalletNavigation.Amount += wallet.Amount;

                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
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
                    var notification1 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch ví",
                        Content = "Giao dịch ví của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch ví",
                        Content = "Giao dịch ví của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);
                }
                else if (choice == (Int32)UpdateTransactionType.Unknown) { return new StatusCodeResult(406); }
            }
            else if (updateStatus == (Int32)VNPayType.CANCELLED)
            {
                if (choice == (Int32)UpdateTransactionType.Booking)
                {
                    wallet.Status = (int)VNPayType.CANCELLED;
                    var booking = _context.BookingTransactions.FirstOrDefault(b => b.BookingTransactionId == wallet.WalletTransactionId);
                    // Check the time when start boooking and when cancle booking (if more 12 hours can't cancle)
                    if (booking.CreatedAt.AddHours(12) < DateTime.UtcNow.AddHours(7))
                    {
                        return new StatusCodeResult(204);
                    }
                    booking.Status = (int)VNPayType.CANCELLED;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.SenderWalletNavigation.PendingAmount += booking.Amount;
                    wallet.SenderWalletNavigation.AvalaibleAmount += booking.Amount;
                    wallet.SenderWalletNavigation.Amount += booking.Amount;

                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.ReceiverWalletNavigation.PendingAmount -= booking.Amount;


                    _context.BookingTransactions.Update(booking);
                    _context.WalletTransactions.Update(wallet);

                    // Find the booking and update the status
                    var book = _context.Bookings
                        .Include(b => b.TutorNavigation)
                        .FirstOrDefault(b => b.BookingId == booking.BookingId);
                    book.Status = (int)BookingEnum.Cancelled;
                    TimeSpan bookingTime = new TimeSpan(book.StudyTime.Value.Hour, book.StudyTime.Value.Minute, 0);
                    // Find the tutor available slot
                    DateTime bookingDate = book.StudyTime.Value.Date;
                    var tutorDateAvailables = _context.TutorDateAvailables
                        .Where(x => x.TutorID == book.TutorId && x.Date.Date == bookingDate)
                        .Select(x => x.TutorDateAvailableID)
                        .ToList();
                    if (tutorDateAvailables == null)
                    {
                        return new StatusCodeResult(452);
                    }
                    var tutorSlotAvailables = _context.TutorSlotAvailables
                        .Where(x => tutorDateAvailables.Contains(x.TutorDateAvailable.TutorDateAvailableID) && x.StartTime == bookingTime)
                        .FirstOrDefault();
                    tutorSlotAvailables.IsBooked = false;
                    tutorSlotAvailables.Status = (Int32)TutorSlotAvailabilityEnum.Available;
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
                    var notification1 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch booking",
                        Content = "Giao dịch booking của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch booking",
                        Content = "Giao dịch booking của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);

                    _context.Bookings.Update(book);
                }
                else if (choice == (Int32)UpdateTransactionType.Course)
                {
                    wallet.Status = (int)VNPayType.REJECT;
                    var course = _context.CourseTransactions.FirstOrDefault(b => b.CourseTransactionId == wallet.WalletTransactionId);
                    course.Status = (int)VNPayType.REJECT;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.SenderWalletNavigation.PendingAmount += course.Amount;
                    wallet.SenderWalletNavigation.AvalaibleAmount += course.Amount;
                    wallet.SenderWalletNavigation.Amount += course.Amount;

                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
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
                    var notification1 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch course",
                        Content = "Giao dịch course của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch course",
                        Content = "Giao dịch course của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);
                }
                else if (choice == (Int32)UpdateTransactionType.Wallet)
                {
                    wallet.Status = (int)VNPayType.REJECT;

                    //update wallet for sender and receiver
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.SenderWalletNavigation.PendingAmount += wallet.Amount;
                    wallet.SenderWalletNavigation.AvalaibleAmount += wallet.Amount;
                    wallet.SenderWalletNavigation.Amount += wallet.Amount;

                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
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
                    var notification1 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch ví",
                        Content = "Giao dịch ví của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = sender.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    var notification2 = new Models.Entities.Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch ví",
                        Content = "Giao dịch ví của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = receiver.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        Status = (int)NotificationEnum.UnRead
                    };
                    _context.Notifications.Add(notification1);
                    _context.Notifications.Add(notification2);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
                    _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);
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
        public async Task<ActionResult<BookingTransaction>> GetBookingTransactionByBookingId(Guid bookingId)
        {
            try
            {
                var bookingTransaction = _context.BookingTransactions.FirstOrDefault(c => c.BookingId == bookingId);
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
        public async Task<ActionResult<List<Models.Entities.CourseTransaction>>> GetAllCourseTransactions()
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
        public async Task<ActionResult<PageResults<Models.Entities.CourseTransaction>>> GetAllCourseTransactionsPaging(PagingRequest request)
        {
            try
            {
                var courseTransactionsList = await _context.CourseTransactions.OrderByDescending(c => c.CreatedAt).ToListAsync();
                if (courseTransactionsList == null || !courseTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedCourseTransactions = PagingHelper<Models.Entities.CourseTransaction>.Paging(courseTransactionsList, request.Page, request.PageSize);
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
        public async Task<ActionResult<Models.Entities.CourseTransaction>> GetCourseTransaction(Guid id)
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
        public async Task<ActionResult<List<Models.Entities.CourseTransaction>>> GetCourseTransactionsBySenderId(Guid id)
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
        public async Task<ActionResult<PageResults<Models.Entities.CourseTransaction>>> GetCourseTransactionBySenderIdPaging(Guid id, PagingRequest request)
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

                var paginatedCourseTransactions = PagingHelper<Models.Entities.CourseTransaction>.Paging(courseTransactionsList, request.Page, request.PageSize);
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
        public async Task<ActionResult<List<Models.Entities.CourseTransaction>>> GetCourseTransactionsByReceiverId(Guid id)
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
        public async Task<ActionResult<PageResults<Models.Entities.CourseTransaction>>> GetCourseTransactionsByReceiverIdPaging(Guid id, PagingRequest request)
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

                var paginatedCourseTransactions = PagingHelper<Models.Entities.CourseTransaction>.Paging(courseTransactionsList, request.Page, request.PageSize);
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
        public async Task<ActionResult<List<Models.Entities.CourseTransaction>>> GetCourseTransactionsByCourseId(Guid id)
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
        public async Task<ActionResult<PageResults<Models.Entities.CourseTransaction>>> GetCourseTransactionsByCourseIdPaging(Guid id, PagingRequest request)
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

                var paginatedCourseTransactions = PagingHelper<Models.Entities.CourseTransaction>.Paging(courseTransactionsList, request.Page, request.PageSize);
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
        // Get Transaction By UserId and Paging
        public async Task<ActionResult<PageResults<WalletTransactionViewVersion2>>>GetCourseTransactionsByUserIdPaging(Guid userId, PagingRequest request)
        {
            try
            {
                var walletTransactionsList = await _context.WalletTransactions
                    .Include(user => user.ReceiverWalletNavigation.UserNavigation)
                    .Include(user => user.SenderWalletNavigation.UserNavigation)
                    .Where(c => c.SenderWalletNavigation.UserId == userId || c.ReceiverWalletNavigation.UserId == userId)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new WalletTransactionViewVersion2
                    {
                        WalletTransactionId = c.WalletTransactionId,
                        SenderId = c.SenderWalletNavigation.UserId,
                        ReceiverId = c.ReceiverWalletNavigation.UserId,
                        SenderName = c.SenderWalletNavigation.UserNavigation.Name,
                        ReceiverName = c.ReceiverWalletNavigation.UserNavigation.Name,
                        Amount = c.Amount,
                        CreatedAt = c.CreatedAt,
                        Status = c.Status,
                        Note = c.Note,
                    })
                    .ToListAsync();
                if(walletTransactionsList == null || !walletTransactionsList.Any())
                {
                    return new StatusCodeResult(404);
                } 
                var pagingWalletTransactions = PagingHelper<WalletTransactionViewVersion2>.Paging(walletTransactionsList, request.Page, request.PageSize);
                if(pagingWalletTransactions == null)
                {
                    return new StatusCodeResult(400);
                }
                return pagingWalletTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        public async Task<ActionResult> CheckAllBookingFinish()
        {
            try
            {
                // Xác định ngày của ngày hôm qua theo múi giờ Hồ Chí Minh
                var timezone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var currentTimeInHoChiMinh = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezone);
                var startDate = currentTimeInHoChiMinh.Date.AddDays(-1);
                var endDate = currentTimeInHoChiMinh.Date; // Ngày hôm nay

                // Truy vấn các giao dịch hoàn thành vào ngày cụ thể
                var bookingTransactions = await _context.BookingTransactions
                    .Include(b => b.BookingNavigation)
                    .Include(b => b.SenderWalletNavigation)
                    .Include(b => b.ReceiverWalletNavigation)
                    .Include(b => b.SenderWalletNavigation.UserNavigation)
                    .Include(b => b.ReceiverWalletNavigation.UserNavigation)
                    .Where(b => b.BookingNavigation.Status == (int)BookingEnum.Finished
                                && b.BookingNavigation.StudyTime >= startDate
                                && b.BookingNavigation.StudyTime < endDate)
                    .ToListAsync();

                if (bookingTransactions == null || !bookingTransactions.Any())
                {
                    return new StatusCodeResult(204);
                }

                foreach (var booking in bookingTransactions)
                {
                    var wallet = await _context.WalletTransactions.FirstOrDefaultAsync(w => w.WalletTransactionId == booking.BookingTransactionId);
                    if (wallet == null)
                    {
                        return new StatusCodeResult(204);
                    }
                    wallet.Status = (int)VNPayType.APPROVE;
                    wallet.SenderWalletNavigation.PendingAmount += booking.Amount;
                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.ReceiverWalletNavigation.PendingAmount -= booking.Amount;
                    wallet.ReceiverWalletNavigation.AvalaibleAmount += booking.Amount;
                    wallet.ReceiverWalletNavigation.Amount += booking.Amount;

                    booking.BookingNavigation.Status = (int)VNPayType.APPROVE;
                    _context.BookingTransactions.Update(booking);
                    _context.WalletTransactions.Update(wallet);
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = booking.SenderWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch booking của bạn đã hoàn thành. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    await _appExtension.SendMail(new MailContent()
                    {
                        To = booking.ReceiverWalletNavigation.UserNavigation.Email,
                        Subject = "Xác nhận giao dịch",
                        Body = "Giao dịch booking của bạn đã hoàn thành. Mã giao dịch: " + wallet.WalletTransactionId
                    });
                    var notification1 = new NotificationDTO
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch booking",
                        Content = "Giao dịch booking của bạn đã hoàn thành. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = booking.SenderWalletNavigation.UserId,
                        CreatedAt = DateTime.UtcNow.AddHours(7)
                    };
                    var notification2 = new NotificationDTO
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Giao dịch booking",
                        Content = "Giao dịch booking của bạn đã hoàn thành. Mã giao dịch: " + wallet.WalletTransactionId,
                        UserId = booking.ReceiverWalletNavigation.UserId,
                        CreatedAt = DateTime.UtcNow.AddHours(7)
                    };

                    Models.Entities.Notification notification1x = _mapper.Map<Models.Entities.Notification>(notification1);
                    Models.Entities.Notification notification2x = _mapper.Map<Models.Entities.Notification>(notification2);
                    _context.Notifications.Add(notification1x);
                    _context.Notifications.Add(notification2x);
                    _firebaseRealtimeDatabaseService.UpdateAsync<NotificationDTO>($"notifications/{notification1.UserId}/{notification1.NotificationId}", notification1);
                    _firebaseRealtimeDatabaseService.UpdateAsync<NotificationDTO>($"notifications/{notification2.UserId}/{notification2.NotificationId}", notification2);
                    await _context.SaveChangesAsync();
                }
                return new StatusCodeResult(200);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }
        public async Task<ActionResult<List<WalletTransaction>>> GetSubscriptionTransactionsOfWalletId(Guid walletId)
        {
            try
            {
                var list = await _context.WalletTransactions.OrderByDescending(c => c.CreatedAt).Where(c => c.Note.Contains("Nâng cấp tài khoản") && c.SenderWalletId == walletId).ToListAsync();
                if(list == null)
                {
                    return new StatusCodeResult(404);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<List<WalletTransaction>>> GetAllSubscriptionTransactions()
        {
            Guid adminWallet = Guid.Parse("E91703BF-5651-4F9A-5D51-08DC93FF5629");
           try
            {
                var list = await _context.WalletTransactions.OrderByDescending(c => c.CreatedAt).Where(c => c.Note.Contains("Nâng cấp tài khoản") && c.ReceiverWalletId == adminWallet).ToListAsync();
                if(list == null)
                {
                    return new StatusCodeResult(404);
                }
                return list.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        // Cancle Booking if before 12h of study time return money to sender and after 12h of study time return money to receiver
        public async Task<IActionResult> CancelBooking(Guid walletTransactionId)
        {
            try
            {
                // Lấy thông tin giao dịch ví từ database
                var wallet = await _context.WalletTransactions.FirstOrDefaultAsync(w => w.WalletTransactionId == walletTransactionId);
                if (wallet == null)
                {
                    return new StatusCodeResult(404);
                }

                // Lấy thông tin booking tương ứng
                var booking = await _context.BookingTransactions
                    .Include(b => b.BookingNavigation)
                    .FirstOrDefaultAsync(b => b.BookingTransactionId == walletTransactionId);
                if (booking == null)
                {
                    return new StatusCodeResult(404);
                }

                // Xác định người gửi và người nhận
                var sender = await _context.Users.Include(u => u.WalletNavigation).FirstOrDefaultAsync(u => u.WalletNavigation.WalletId == wallet.SenderWalletId);
                var receiver = await _context.Users.Include(u => u.WalletNavigation).FirstOrDefaultAsync(u => u.WalletNavigation.WalletId == wallet.ReceiverWalletId);

                // Cập nhật trạng thái giao dịch ví và booking
                if (booking.BookingNavigation.StudyTime.Value.AddHours(-12) > DateTime.UtcNow.AddHours(7))
                {
                    // Xử lý hoàn tiền
                    wallet.Status = (int)VNPayType.APPROVE;
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.SenderWalletNavigation.AvalaibleAmount += booking.Amount;
                    wallet.SenderWalletNavigation.Amount += booking.Amount;
                    wallet.SenderWalletNavigation.PendingAmount += booking.Amount;

                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.ReceiverWalletNavigation.PendingAmount -= booking.Amount;
                    booking.BookingNavigation.Status = (int)BookingEnum.Cancelled;

                    await SendNotificationAndEmail(sender, receiver, "Giao dịch booking của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId);
                }
                else
                {
                    // Xử lý hủy booking mà không hoàn tiền
                    wallet.Status = (int)VNPayType.APPROVE;
                    //update wallet for sender and receiver     
                    wallet.SenderWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.SenderWalletNavigation.PendingAmount += booking.Amount;
                    wallet.ReceiverWalletNavigation.Amount += booking.Amount;
                    wallet.ReceiverWalletNavigation.LastBalanceUpdate = DateTime.UtcNow.AddHours(7);
                    wallet.ReceiverWalletNavigation.AvalaibleAmount += booking.Amount;
                    wallet.ReceiverWalletNavigation.PendingAmount -= booking.Amount;
                    booking.BookingNavigation.Status = (int)BookingEnum.Cancelled;

                    await SendNotificationAndEmail(sender, receiver, "Giao dịch booking của bạn đã được hủy bỏ. Mã giao dịch: " + wallet.WalletTransactionId);
                }
                
                _context.WalletTransactions.Update(wallet);
                _context.BookingTransactions.Update(booking);

                var book = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == booking.BookingId);
                if (book != null)
                {
                    book.Status = (int)BookingEnum.Cancelled;
                    _context.Bookings.Update(book);
                }
                await _context.SaveChangesAsync();
                return new OkResult();
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        // Send Notification 
        private async Task SendNotificationAndEmail(User sender, User receiver, string content)
        {
            await _appExtension.SendMail(new MailContent()
            {
                To = sender.Email,
                Subject = "Xác nhận giao dịch",
                Body = content
            });
            await _appExtension.SendMail(new MailContent()
            {
                To = receiver.Email,
                Subject = "Xác nhận giao dịch",
                Body = content
            });

            var notification1 = new Models.Entities.Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = "Giao dịch booking",
                Content = content,
                UserId = sender.Id,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (int)NotificationEnum.UnRead
            };
            var notification2 = new Models.Entities.Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = "Giao dịch booking",
                Content = content,
                UserId = receiver.Id,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Status = (int)NotificationEnum.UnRead
            };
            _context.Notifications.Add(notification1);
            _context.Notifications.Add(notification2);

            await _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{sender.Id}/{notification1.NotificationId}", notification1);
            await _firebaseRealtimeDatabaseService.UpdateAsync<Models.Entities.Notification>($"notifications/{receiver.Id}/{notification2.NotificationId}", notification2);
        }


    }
}
