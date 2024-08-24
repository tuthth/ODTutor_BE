using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class StudentSubscriptionRequest
    {
        public string StudentNameSubscription { get; set; }
        public String Description { get; set; }
        public decimal Price { get; set; }
        public int Types { get; set; }
        public List<string> PackageDescriptions { get; set; }
    }
    public class StudentSubscriptionDetail
    {
        public string Description { get; set; }
    }
    public class StudentSubscription : StudentSubscriptionRequest
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<StudentSubscriptionDetail> SubscriptionDetails { get; set; }
        public int NumberOfSubscriptions { get; set; }
        public int Status { get; set; }
    }
}
    