using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    /// <summary>
    /// Exception to be used when an argument has an empty value (but not null)
    /// e.g. String.Emtpy.
    /// </summary>
    public class ArgumentEmptyException : ArgumentException
    {
        public ArgumentEmptyException()
        {
        }

        public ArgumentEmptyException(string propertyName)
            : base("Value cannot be empty. Property name: " + propertyName, propertyName)
        {
        }
    }
}
