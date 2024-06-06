using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class UserBlockView
    {
        public Guid CreateUserId { get; set; }
        public Guid TargetUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
