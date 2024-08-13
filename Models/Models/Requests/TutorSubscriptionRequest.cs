using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorSubscriptionRequest
    {
        public string TutorNameSubscription { get; set; }
        public String Description { get; set; }
        public decimal Price { get; set; }
        public int Types { get; set; }
    }
}
