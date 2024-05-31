using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enumerables
{
    public enum ReportEnum
    {
        Unknown,
        Finished,
        Processing,
        Cancelled
    }
    public enum ReportActionEnum
    {
        Unknown,
        SevenDays,
        ThirtyDays,
        NinetyDays,
        Lifetime
    }
}
