using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class WalletTransaction
    {
        public Guid WalletId { get; set; }
        public Guid BookingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }

        public virtual Wallet? WalletNavigation {  get; set; }
    }
}
