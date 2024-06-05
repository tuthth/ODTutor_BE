using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class UserInteractRequest
    {
        public Guid CreateUserId { get; set; }
        public Guid TargetUserId { get; set; }
    }
}
