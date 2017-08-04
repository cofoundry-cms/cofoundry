using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A Page Template represents a physical view template file and is used
    /// by a Page to render out content. This summary object is intended to
    /// be used in lists/searches and contains aggregate information.
    /// </summary>
    public class PageTemplateSummary
    {
        /// <summary>
        /// The database id of the template
        /// </summary>
        public int PageTemplateId { get; set; }

        /// <summary>
        /// File name excluding extension and any leading underscores
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Human readable display name. This is unique, so to avoid 
        /// confusion when selecting a template from a list
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Long description, nullable and can be empty
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Pages can be one of several types represented by 
        /// the PageType enum which will either be PageType.Generic or
        /// one of the special page function like PageType.CustomEntityDetails
        /// or PageType.NotFound
        /// </summary>
        public PageType PageType { get; set; }

        /// <summary>
        /// The total number of regions defined in this template
        /// </summary>
        public int NumRegions { get; set; }

        /// <summary>
        /// The number of pages that use this template
        /// </summary>
        public int NumPages { get; set; }

        /// <summary>
        /// Indicates if the template has been archived and is no longer available 
        /// to be used in new Pages. An archived template may still be used in an 
        /// active page to support scenarios where you need to transition between
        /// an old template to a new template.
        /// </summary>
        public bool IsArchived { get; set; }

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
