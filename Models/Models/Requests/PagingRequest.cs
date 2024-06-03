using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class PagingRequest
    {
        [Range (1, int.MaxValue, ErrorMessage ="Only positive number allowed")]   
        public int Page { get; set; } = 1;
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int PageSize { get; set; } = 10;
    }
}
