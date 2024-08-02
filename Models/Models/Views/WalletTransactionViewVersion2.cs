using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class WalletTransactionViewVersion2
    {
        public Guid WalletTransactionId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public string? Note { get; set; }
    }
}
