using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enumerables
{
    public enum TutorEnum
    {
        Pending = 0,
        Active = 1,
        Inactive = 2,
        Banned = 3
    }
    public enum TutorSlotAvailabilityEnum
    {   
        Available = 0,
        NotAvailable = 1,
        Cancelled = 2,
    }
}
