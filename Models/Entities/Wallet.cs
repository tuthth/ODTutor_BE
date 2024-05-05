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
        public DateTime LastBalanceUpdate { get; set; } = DateTime.Now;
        public int Status { get; set; }
        public virtual ICollection<BookingTransaction>? BookingTransactionsNavigation { get; set; }
        public virtual ICollection<WalletTransaction>? WalletTransactionsNavigation { get; set; }
        public virtual User? UserNavigation { get; set; }
    }
}
