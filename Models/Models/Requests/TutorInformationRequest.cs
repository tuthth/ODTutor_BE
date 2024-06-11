using Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TutorInformationRequest
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string IdentityNumber { get; set; }
        [Required]
        public string VideoUrl { get; set; }
    }
}
