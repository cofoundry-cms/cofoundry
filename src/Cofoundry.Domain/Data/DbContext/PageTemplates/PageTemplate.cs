using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class PageTemplate : ICreateAuditable
    {
        #region constructor

        public PageTemplate()
        {
            PageTemplateSections = new List<PageTemplateSection>();
            PageVersions = new List<PageVersion>();
        }

        #endregion

        public int PageTemplateId { get; set; }

        public int PageTypeId { get; set; }

        /// <summary>
        /// Filename excluding extension and any leading underscores.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Full path to the view file including the filename
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Long description, not nullable but can be empty.
        /// </summary>
        public string Description { get; set; }

        public string CustomEntityDefinitionCode { get; set; }

        public string CustomEntityModelType { get; set; }

        public virtual CustomEntityDefinition CustomEntityDefinition { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }

        public int CreatorId { get; set; }

        public virtual User Creator { get; set; }

        #endregion

        #region collections

        public virtual ICollection<PageTemplateSection> PageTemplateSections { get; set; }

        public virtual ICollection<PageVersion> PageVersions { get; set; }

        #endregion

        #region helpers

        public bool IsCustomEntityTemplate()
        {
            return !string.IsNullOrWhiteSpace(CustomEntityDefinitionCode);
        }

        #endregion


    }
}
