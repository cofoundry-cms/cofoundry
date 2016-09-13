using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class PageTemplateSection : ICreateAuditable
    {
        public PageTemplateSection()
        {
            PageVersionModules = new List<PageVersionModule>();
            PageModuleTypes = new List<PageModuleType>();
        }

        public int PageTemplateSectionId { get; set; }
        public int PageTemplateId { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether this section should apply to the Page (false) or
        /// to a CustomEntity (true). This is only relevant for Templates with 
        /// a type of CustomEntityDetails
        /// </summary>
        public bool IsCustomEntitySection { get; set; }

        public virtual PageTemplate PageTemplate { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }

        public int CreatorId { get; set; }

        public virtual User Creator { get; set; }

        #endregion

        public virtual ICollection<PageVersionModule> PageVersionModules { get; set; }
        public virtual ICollection<PageModuleType> PageModuleTypes { get; set; }
    }
}
