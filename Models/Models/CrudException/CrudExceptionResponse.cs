using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.CrudException
{
    public class CrudExceptionResponse
    {
        public string Type { get; set;}
        public string Title { get; set;}
        public int Status { get; set; }
        public string TraceId { get; set; }
        public IDictionary<string, string[]> Errors { get; set; }
    }
}
