using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class VisualEditorSettings : ICofoundrySettings
    {
        /// <summary>
        /// This needs to be removed at some point - it's a global setting for the device view
        /// which affects every admin looking at pages in the site.
        /// </summary>
        public string VisualEditorDeviceView { get; set; }
    }
}
