using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CofoundryAdminUserArea : IUserAreaDefinition
    {
        public static string AreaCode = "COF";

        public string UserAreaCode
        {
            get { return AreaCode; }
        }

        public string Name
        {
            get { return "Cofoundry"; }
        }

        public bool AllowPasswordLogin
        {
            get { return true; }
        }

        public bool UseEmailAsUsername
        {
            get { return true; }
        }

        public string LoginPath
        {
            get { return "/admin/auth/login"; }
        }

        public string LogoutPath
        {
            get { return null; }
        }

        public string AccessDeniedPath
        {
            get { return null; }
        }
    }
}
