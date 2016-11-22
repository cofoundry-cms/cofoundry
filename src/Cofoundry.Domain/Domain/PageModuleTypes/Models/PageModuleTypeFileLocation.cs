using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the location of a view file for
    /// a page module type.
    /// </summary>
    public class PageModuleTypeFileLocation
    {
        /// <summary>
        /// The file name (without extension) which is basically the unique 
        /// identifier for this page module type
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The virtual path to the view file
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Locations of any alternative template view files, indexed by FileName 
        /// </summary>
        public Dictionary<string, PageModuleTypeTemplateFileLocation> Templates { get; set; }
    }
}
