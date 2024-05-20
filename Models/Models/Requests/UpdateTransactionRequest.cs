using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class UpdateTransactionRequest
    {
        public Guid TransactionId { get; set; }
        public int Choice { get; set; }
        public int UpdateStatus { get; set; }
    }
}
