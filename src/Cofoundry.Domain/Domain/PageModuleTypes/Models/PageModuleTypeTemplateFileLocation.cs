using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the location of a view file for
    /// a page module type template.
    /// </summary>
    public class PageModuleTypeTemplateFileLocation
    {
        /// <summary>
        /// The file name (without extension) which is basically the unique 
        /// identifier for this template
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The virtual path to the template view file
        /// </summary>
        public string Path { get; set; }
    }
}
