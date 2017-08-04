using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the location of a view file for
    /// a page block type template.
    /// </summary>
    public class PageBlockTypeTemplateFileLocation
    {
        /// <summary>
        /// The file name (without extension) which is used as the unique 
        /// identifier for this template
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The virtual path to the template view file
        /// </summary>
        public string Path { get; set; }
    }
}
