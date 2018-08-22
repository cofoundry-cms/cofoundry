using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    public static class DbConstants
    {
        /// <summary>
        /// The schema for Cofoundry tables 'Cofoundry'
        /// </summary>
        public static string CofoundrySchema = "Cofoundry";

        /// <summary>
        /// The schema for Cofoundry plugin tables 'CofoundryPlugin'
        /// </summary>
        public static string CofoundryPluginSchema = "CofoundryPlugin";

        /// <summary>
        /// The default/suggested schema for a cofoundry implementation's tables 'app'
        /// </summary>
        public static string DefaultAppSchema = "app";
    }
}
