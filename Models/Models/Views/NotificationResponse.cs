using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class NotificationResponse
    {
        public Guid UserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
