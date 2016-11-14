using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used in the UpdatePageTemplateRegistrationCommandHandler when there's
    /// a problem with the registration of page module types that cannot be 
    /// resolved automatically.
    /// </summary>
    public class PageModuleTypeRegistrationException : Exception
    {
        public PageModuleTypeRegistrationException()
        {
        }

        public PageModuleTypeRegistrationException(string message)
            : base(message)
        {
        }
    }
}
