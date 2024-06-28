using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Validation
{
    public class DateOfBirthValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime date)
            {
                return date < DateTime.UtcNow.AddHours(7);
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be a date in the past.";
        }
    }
}
