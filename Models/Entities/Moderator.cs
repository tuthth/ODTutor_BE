using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Moderator
    {
        public Guid ModeratorId { get; set; }
        public Guid UserId { get; set; }
        public virtual User? UserNavigation { get; set; }
        public virtual ICollection<TutorAction>? TutorActionsNavigation { get; set; }
    }
}
