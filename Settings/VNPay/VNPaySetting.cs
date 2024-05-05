using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings.VNPay
{
    public class VNPaySetting
    {
        public string VnPay_TmnCode { get; set; } = null!;
        public string VnPay_HashSecret { get; set; } = null!;
        public string VnPay_Url { get; set; } = null!;
    }
}
