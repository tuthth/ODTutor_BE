using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class SubjectAddNewRequest
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Note { get; set; }
    }
    public class  UpdateSubject : SubjectAddNewRequest
    {
        public Guid SubjectId { get; set; }
    }
}
