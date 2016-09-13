using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class UserAreaMicroSummary
    {
        /// <summary>
        /// 3 letter code identifying this user area.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Display name of the area, used in the Cofoundry admin panel
        /// </summary>
        public string Name { get; set; }
    }
}
