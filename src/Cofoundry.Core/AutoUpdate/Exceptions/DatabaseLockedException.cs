using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    public class DatabaseLockedException : Exception
    {
        public DatabaseLockedException()
            : base("The database has been locked to prevent accidental automatic updates. Please unlock it before pushing updates.")
        {
        }
    }
}
