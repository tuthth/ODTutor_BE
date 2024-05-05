using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Transaction
    {
        public long Id { get; set; }
        public int? WalletId { get; set; }
        public string? Type { get; set; }
        public decimal? Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Wallet? WalletNavigation { get; set; }
    }
}
