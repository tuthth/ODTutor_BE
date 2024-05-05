﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class CourseOutline
    {
        public Guid CourseOutlineId { get; set; }
        public Guid CourseId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }

        public virtual Course? CoursesNavigation { get; set; }
    }
}
