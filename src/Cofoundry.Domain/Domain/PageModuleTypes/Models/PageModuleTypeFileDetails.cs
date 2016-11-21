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
        /// Information about each of the sections found by parsing the 
        /// file. Partial files are also scanned as part fo the process 
        /// so sections can be included in these too.
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
