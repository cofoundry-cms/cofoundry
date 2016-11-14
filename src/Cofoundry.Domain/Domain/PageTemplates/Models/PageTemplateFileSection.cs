using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Each PageTemplate can have zero or more sections which are defined in the 
    /// template file using the CofoundryTemplate helper, 
    /// e.g. @Cofoundry.Template.Section("MySectionName"). These sections represent
    /// areas where page modules can be placed (i.e. insert content). This object 
    /// represents one of those section scanned from the template file.
    /// </summary>
    public class PageTemplateFileSection
    {
        /// <summary>
        /// Section names can be any text string but will likely be 
        /// alpha-numerical human readable names like 'Heading', 'Main Content'.
        /// Section names should be unique (non-case sensitive) irrespective of
        /// whether thy are custom entity sections or not.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Will only apply for custom entity details templates. True indicates 
        /// this section should apply to every custom entity, false indicates it 
        /// applies to page itself (across all custom entity variations).
        /// </summary>
        public bool IsCustomEntitySection { get; set; }
    }
}
