using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class WalletView
    {
        public Guid WalletId { get; set; }
        public Guid UserId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AvalaibleAmount { get; set; }
        public decimal? PendingAmount { get; set; }
        public DateTime LastBalanceUpdate { get; set; } = DateTime.UtcNow.AddHours(7);
        public int Status { get; set; }
    }
}
