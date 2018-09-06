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
        public ICollection<PageBlockTypeTemplateFileDetails> Templates { get; set; }

        /// <summary>
        /// A human readable name that is displayed when selecting block types.
        /// The name should ideally be unique but this is not enforced as long as
        /// the filename is unique.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An optional description used to help users pick a block
        /// type from a list of options.
        /// </summary>
        public string Description { get; set; }
    }
}
