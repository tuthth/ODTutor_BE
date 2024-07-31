using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enumerables
{
    public enum VNPayTransactionType
    {
        Unknown,
        Deposit,
        Withdraw,
        Upgrade,
        StudentSubscription,
        TutorExperienceSubscription,
        TutorSubscription,
    }
}
