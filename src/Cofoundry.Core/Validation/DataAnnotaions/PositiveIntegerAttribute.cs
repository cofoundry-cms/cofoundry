using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Validation
{
    /// <summary>
    /// Validates the property is a positive integer value. Useful
    /// for making sure an Id/Key field has been entered.
    /// </summary>
    public class PositiveIntegerAttribute : RangeAttribute
    {
        public PositiveIntegerAttribute() :
            base(1, Int32.MaxValue)
        {
            ErrorMessage = "The {0} field must be a positive integer value.";
        }
    }
}
