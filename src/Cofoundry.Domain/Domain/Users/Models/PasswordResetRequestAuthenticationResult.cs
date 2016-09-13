using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PasswordResetRequestAuthenticationResult
    {
        public bool IsValid { get; set; }

        public string ValidationErrorMessage { get; set; }
    }
}
