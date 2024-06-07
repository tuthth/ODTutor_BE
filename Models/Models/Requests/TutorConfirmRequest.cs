using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorConfirmRequest
    {
        public Guid TutorID { get; set; }
        public decimal Price { get; set; }
    }
}
