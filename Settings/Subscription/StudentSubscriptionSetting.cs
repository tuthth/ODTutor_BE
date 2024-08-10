using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings.Subscription
{
    public class StudentSubscriptionSetting
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int RequestCreatePerDay { get; set; }
        public int ContactPerDay { get; set; }
        public string ViewType { get; set; }
    }
}
