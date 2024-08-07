using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enumerables
{
    public enum CourseEnum
    {
        Unknown = 0,
        Active = 1,
        Inactive = 2,
        Deleted = 3,
        Finished = 4,
        InProgress = 5
    }
    public enum StudentCourseEnum
    {
        Success = 1,
        Failed = 2,
        Canceled = 3
    }
}
