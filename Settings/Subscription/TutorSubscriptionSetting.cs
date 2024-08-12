using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings.Subscription
{
    public class TutorSubscriptionSetting
    {
        public string Name { get; set; }
        public string TutorNameSubscription { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeployAt { get; set; } // Ensure this matches the property name in the method
        public int? NumberOfUser { get; set; } // Ensure this matches the property name in the method
        public List<string> MutualDescriptions { get; set; }
        public List<string> PrivateDescriptions { get; set; }
    }
}
