using Models.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorSubscriptionViewResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> SubscriptionDetails { get; set; }
        public int NumberOfSubscriptions { get; set; }
        public int Status { get; set; }
        public string TutorNameSubscription { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Types { get; set; }
        public List<TutorDataBoughtSubscription>? tutorDataBoughtSubscriptions { get; set; }
    }

    public class TutorDataBoughtSubscription
    {
        public Guid TutorId { get; set; }
        public string TutorName { get; set; }
        public string TutorAvatar { get; set; }
        public int SubscriptionType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}
