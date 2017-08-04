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
    /// areas where page blocks can be placed (i.e. insert content).
    /// </summary>
    public class PageTemplateRegionDetails
    {
        /// <summary>
        /// The database id of the region
        /// </summary>
        public int PageTemplateRegionId { get; set; }

        /// <summary>
        /// The id of the page template this region is parented to
        /// </summary>
        public int PageTemplateId { get; set; }

        /// <summary>
        /// Region names can be any text string but will likely be 
        /// alpha-numerical human readable names like 'Heading', 'Main Content'.
        /// Region names should be unique (non-case sensitive) irrespective of
        /// whether thy are custom entity regions or not.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether this region should apply to the Page (false) or
        /// to a CustomEntity (true). This is only relevant for Templates with 
        /// a type of PageType.CustomEntityDetails
        /// </summary>
        public bool IsCustomEntityRegion { get; set; }

        #region Auditing

        /// <summary>
        /// The date the template was created
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The date the template was last updated
        /// </summary>
        public DateTime UpdateDate { get; set; }

        #endregion
    }
}
