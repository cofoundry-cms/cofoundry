using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Each PageTemplate can have zero or more regions which are defined in the 
    /// template file using the CofoundryTemplate helper, 
    /// e.g. @Cofoundry.Template.Region("MyRegionName"). These regions represent
    /// areas where page blocks can be placed (i.e. insert content). This object 
    /// represents one of those regions scanned from the template file.
    /// </summary>
    public class PageTemplateFileRegion
    {
        /// <summary>
        /// Region names can be any text string but will likely be 
        /// alpha-numerical human readable names like 'Heading', 'Main Content'.
        /// Region names should be unique (non-case sensitive) irrespective of
        /// whether thy are custom entity regions or not.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Will only apply for custom entity details templates. True indicates 
        /// this region should apply to every custom entity, false indicates it 
        /// applies to page itself (across all custom entity variations).
        /// </summary>
        public bool IsCustomEntityRegion { get; set; }
    }
}
