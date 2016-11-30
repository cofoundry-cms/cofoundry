using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Each PageTemplate can have zero or more sections which are defined in the 
    /// template file using the CofoundryTemplate helper, 
    /// e.g. @Cofoundry.Template.Section("MySectionName"). These sections represent
    /// areas where page modules can be placed (i.e. insert content).
    /// </summary>
    public partial class PageTemplateSection
    {
        public PageTemplateSection()
        {
            PageVersionModules = new List<PageVersionModule>();
        }

        /// <summary>
        /// The database id of the section
        /// </summary>
        public int PageTemplateSectionId { get; set; }

        /// <summary>
        /// The id of the page tmeplate this section is parented to
        /// </summary>
        public int PageTemplateId { get; set; }

        /// <summary>
        /// Section names can be any text string but will likely be 
        /// alpha-numerical human readable names like 'Heading', 'Main Content'.
        /// Section names should be unique (non-case sensitive) irrespective of
        /// whether thy are custom entity sections or not.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether this section should apply to the Page (false) or
        /// to a CustomEntity (true). This is only relevant for Templates with 
        /// a type of PageType.CustomEntityDetails
        /// </summary>
        public bool IsCustomEntitySection { get; set; }

        /// <summary>
        /// The page tmeplate this section is parented to
        /// </summary>
        public virtual PageTemplate PageTemplate { get; set; }

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

        /// <summary>
        /// In each page implementation a section can have zero or more page 
        /// modules, these contain the dynamic content that gets rendered into
        /// the template.
        /// </summary>
        public virtual ICollection<PageVersionModule> PageVersionModules { get; set; }
    }
}
