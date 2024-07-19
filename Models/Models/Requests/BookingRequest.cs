using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class BookingRequest
    {
        public Guid StudentId { get; set; }
        public Guid TutorId { get; set; }
        public Guid TutorSlotAvalaibleID { get; set; }
    }
    public class UpdateBookingRequest : BookingRequest
    {
        public Guid BookingId { get; set; }
    }
}
