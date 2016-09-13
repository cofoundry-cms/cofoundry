using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class InternalSettings : ICofoundrySettings
    {
        /// <summary>
        /// Indicates whether the site setup has been completed successfully.
        /// </summary>
        public bool IsSetup { get; set; }
    }
}
