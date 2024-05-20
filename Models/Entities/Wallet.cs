using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Wallet
    {
        public Guid WalletId { get; set; }
        public Guid UserId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AvalaibleAmount { get; set; }
        public decimal? PendingAmount { get; set; }
        public DateTime LastBalanceUpdate { get; set; } = DateTime.UtcNow;
        public int Status { get; set; }
        public virtual ICollection<BookingTransaction>? SenderBookingTransactionsNavigation { get; set; }
        public virtual ICollection<BookingTransaction>? ReceiverBookingTransactionsNavigation { get; set; }
        public virtual ICollection<WalletTransaction>? SenderWalletTransactionsNavigation { get; set; }
        public virtual ICollection<WalletTransaction>? ReceiverWalletTransactionsNavigation { get; set; }
        public virtual ICollection<CourseTransaction>? SenderCourseTransactionsNavigation { get; set; }
        public virtual ICollection<CourseTransaction>? ReceiverCourseTransactionsNavigation { get; set; }
        public virtual User? UserNavigation { get; set; }
    }
}
