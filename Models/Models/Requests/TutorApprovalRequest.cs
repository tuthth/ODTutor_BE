﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorApprovalRequest
    {
        public Guid TutorActionId { get; set; }
        public Guid ApprovalID { get; set; }
    }
}
