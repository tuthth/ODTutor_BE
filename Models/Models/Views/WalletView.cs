using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class WalletView
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }
        public decimal? Balance { get; set; }
        public DateTime LastBalanceUpdate { get; set; } = DateTime.Now;
    }
}
