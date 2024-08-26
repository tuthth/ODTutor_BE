using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class BookingRequestV2
    {
        public Guid StudentId { get; set; }
        public Guid TutorId { get; set; }
        public Guid TutorSlotAvalaibleID { get; set; }
        public Guid TutorSubjectID { get; set; }
        public string? BookingContent { get; set; }
    }
    public class UpdateBookingRequestV2 : BookingRequestV2
    {
        public Guid BookingId { get; set; }
    }
}
