using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class MessageRequest
    {
        public string CollectionName { get; set; }
        public string DocumentName { get; set; }
        public string Message { get; set; }
    }
}
