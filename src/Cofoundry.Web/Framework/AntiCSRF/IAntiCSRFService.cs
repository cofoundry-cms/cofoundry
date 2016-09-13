using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Cofoundry.Web
{
    public interface IAntiCSRFService
    {
        string GetToken();

        void ValidateToken(string token);
    }
}
