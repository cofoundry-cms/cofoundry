using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the result of parsing a page block view file
    /// for information.
    /// </summary>
    public class PageBlockTypeFileDetails
    {
        /// <summary>
        /// Information about any alternative template files 
        /// </summary>
        public IEnumerable<PageBlockTypeTemplateFileDetails> Templates { get; set; }

        /// <summary>
        /// The display name for the block in the administration UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the block in the administration UI
        /// </summary>
        public string Description { get; set; }
    }
}
