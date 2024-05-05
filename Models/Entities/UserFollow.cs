using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class UserFollow
    {
        public Guid CreateUserId { get; set; }
        public Guid TargetUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User? CreateUserNavigation { get; set; }
        public virtual User? TargetUserNavigation { get; set;}
    }
}
