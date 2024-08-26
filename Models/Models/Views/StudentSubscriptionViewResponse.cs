using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class StudentSubscriptionViewResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> SubscriptionDetails { get; set; }
        public int NumberOfSubscriptions { get; set; }
        public int Status { get; set; }
        public string StudentNameSubscription { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Types { get; set; }
        public List<StudentDataBoughtSubscription>? studentDataBoughtSubscriptions { get; set; }
    }

    public class StudentDataBoughtSubscription
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentAvatar { get; set; }
        public int SubscriptionType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
