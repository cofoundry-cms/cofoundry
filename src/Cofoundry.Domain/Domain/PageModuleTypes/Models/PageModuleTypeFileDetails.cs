using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the result of parsing a page module view file
    /// for information.
    /// </summary>
    public class PageModuleTypeFileDetails
    {
        /// <summary>
        /// Information about any alternative template files 
        /// </summary>
        public IEnumerable<PageModuleTypeTemplateFileDetails> Templates { get; set; }

        /// <summary>
        /// The display name for the module in the administration UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the module in the administration UI
        /// </summary>
        public string Description { get; set; }
    }
}
