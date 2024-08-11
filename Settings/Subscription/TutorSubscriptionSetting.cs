﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings.Subscription
{
    public class TutorSubscriptionSetting
    {   
        public string Name {get; set;}
        public string TutorNameSubscription { get; set; }
        public String Description { get; set; }
        public decimal Price {get; set;}
        public int Type {get; set;}
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<String> MutualDescriptions { get; set; }
        public List<String> PrivateDescriptions { get; set; }
    }
}
