using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Service for generating and validation tokens to prevent
    /// cross site request forgery attacks.
    /// </summary>
    public interface IAntiCSRFService
    {
        string GetToken();

        void ValidateToken(string token);
    }
}
