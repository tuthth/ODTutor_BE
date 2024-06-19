using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class TutorRegisterStep1Response
    {
        public string?imageUrl { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? identifyNumber { get; set; }
        public List<string>? Subjects { get; set; }
        public string? videoUrl { get; set; }    
    }
}
