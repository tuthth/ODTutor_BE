using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enumerables
{
    public enum BookingEnum
    {
        WaitingPayment = 0,
        Learning = 1,
        Finished = 2,
        Cancelled = 3,
        Success = 4,
        WaittingConfirmRescheduleForStudent = 5,
        WaittingConfirmRescheduleForTutor = 6,
    }
}
