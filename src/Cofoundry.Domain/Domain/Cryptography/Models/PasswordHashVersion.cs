using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public enum PasswordHashVersion
    {
        /// <summary>
        /// Original version (SHA1, 4 byte salt)
        /// </summary>
        V1 = 1,
        /// <summary>
        /// Updated version SHA1, 24byte salt, 64000 iterations
        /// </summary>
        V2 = 2
    }
}
