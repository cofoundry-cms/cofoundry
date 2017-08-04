using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used in RegisterPageBlockTypesCommandHandler when there's
    /// a problem with the registration of page block types that cannot be 
    /// resolved automatically.
    /// </summary>
    public class PageBlockTypeRegistrationException : Exception
    {
        public PageBlockTypeRegistrationException()
        {
        }

        public PageBlockTypeRegistrationException(string message)
            : base(message)
        {
        }
    }
}
