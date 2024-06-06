using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class BookingTransactionView
    {
        public Guid BookingTransactionId { get; set; }
        public Guid SenderWalletId { get; set; }
        public Guid ReceiverWalletId { get; set; }
        public Guid BookingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
    }
}
