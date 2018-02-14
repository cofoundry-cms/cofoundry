using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the result of parsing a page template view file
    /// for information.
    /// </summary>
    public class PageTemplateFileInfo
    {
        /// <summary>
        /// Information about each of the regions found by parsing the 
        /// file. Partial files are also scanned as part of the process 
        /// so regions can be included in these too.
        /// </summary>
        public ICollection<PageTemplateFileRegion> Regions { get; set; }

        /// <summary>
        /// If the template file is for a custom entity details page the
        /// view model will be CustomEntityPageViewModel&lt;TDataModel&gt;
        /// and TDataModel will be extracted and placed into this property.
        /// </summary>
        public string CustomEntityModelType { get; set; }

        /// <summary>
        /// Wether this a generic page or has some kind of special function
        /// e.g. NotFound, CustomEntityDetails.
        /// </summary>
        public PageType PageType { get; set; }

        /// <summary>
        /// If the template file is for a custom entity details page the
        /// definition is extracted and placed here.
        /// </summary>
        public CustomEntityDefinitionMicroSummary CustomEntityDefinition { get; set; }

        /// <summary>
        /// An optional paragraph-length description of the template
        /// </summary>
        public string Description { get; set; }
    }
}
