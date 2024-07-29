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
        Inprocessing = 2,
        Denny = 3,
        Banned = 4,
        Paused = 5,
    }
    public enum TutorSlotAvailabilityEnum
    {
        Available = 0,
        NotAvailable = 1,
        Cancelled = 2,
    }
    public enum TutorSubjectEnum
    {
        Available = 0,
        NotAvailable = 1,
    }

    public enum TutorPackageEnum
    {
        Standard = 0,
        Premium = 1,
    }
}
