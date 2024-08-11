using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enumerables
{
    public class TutorSubscriptionEnum
    {
        public enum TutorSubscriptionsEnum
        {
            Free = 0,
            Basic = 1,
            Premium = 2,
        }

        // Status of the subscription
        public enum TutorSubscriptionStatusEnum
        {
            Active = 0,
            Inactive = 1,
        }
    }
}
