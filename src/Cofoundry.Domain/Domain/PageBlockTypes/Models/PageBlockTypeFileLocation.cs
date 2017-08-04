using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the location of a view file for
    /// a page block type.
    /// </summary>
    public class PageBlockTypeFileLocation
    {
        /// <summary>
        /// The file name (without extension) which is used as the unique 
        /// identifier for this page block type.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The virtual path to the view file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Locations of any alternative template view files, indexed by FileName.
        /// </summary>
        public Dictionary<string, PageBlockTypeTemplateFileLocation> Templates { get; set; }
    }
}
