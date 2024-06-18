using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorActionView
    {
        public Guid TutorActionId { get; set; }
        public Guid TutorId { get; set; }
        public Guid? ModeratorId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? ReponseDate { get; set; }
        public string? MeetingLink { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ActionType { get; set; }
        public int Status { get; set; }
    }
}
