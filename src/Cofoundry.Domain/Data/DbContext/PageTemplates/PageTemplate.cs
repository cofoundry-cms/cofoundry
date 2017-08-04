using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// A Page Template represents a physical view template file and is used
    /// by a Page to render out content. 
    /// </summary>
    public partial class PageTemplate
    {
        #region constructor

        public PageTemplate()
        {
            PageTemplateRegions = new List<PageTemplateRegion>();
            PageVersions = new List<PageVersion>();
        }

        #endregion

        /// <summary>
        /// The database id of the template
        /// </summary>
        public int PageTemplateId { get; set; }

        /// <summary>
        /// Pages can be one of several types represented in the domain by 
        /// the PageType enum which will either be PageType.Generic or
        /// one of the special page function like PageType.CustomEntityDetails
        /// or PageType.NotFound
        /// </summary>
        public int PageTypeId { get; set; }

        /// <summary>
        /// File name excluding extension and any leading underscores
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Full virtual path to the view file including the filename. This must
        /// be unique
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Human readable display name. This must be unique to avoid 
        /// confusion when selecting a template from a list
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Long description, nullable and can be empty
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// If this template is of type PageType.CustomEntityDetails then 
        /// it will represent the details page of a specific custom entity
        /// definition.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// If this template is of type PageType.CustomEntityDetails, this will
        /// be the type of the custom entity model used in the template e.g.
        /// if the template model is CustomEntityPageViewModel&lt;BlogPostDetailsDisplayModel&gt;
        /// this property would be BlogPostDetailsDisplayModel.
        /// </summary>
        public string CustomEntityModelType { get; set; }

        /// <summary>
        /// If this template is of type PageType.CustomEntityDetails then 
        /// it will represent the details page of a specific custom entity
        /// definition.
        /// </summary>
        public virtual CustomEntityDefinition CustomEntityDefinition { get; set; }

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

        #region collections

        /// <summary>
        /// Each template can have zero or more regions which are defined in the 
        /// template file using the CofoundryTemplate helper, 
        /// e.g. @Cofoundry.Template.Region("MyRegionName"). These regions represent
        /// areas where page blocks can be placed (i.e. insert content).
        /// </summary>
        public virtual ICollection<PageTemplateRegion> PageTemplateRegions { get; set; }

        /// <summary>
        /// Each page template can be attached to a page via a page version. This is so that 
        /// the template can be changed without having to re-create the page.
        /// </summary>
        public virtual ICollection<PageVersion> PageVersions { get; set; }

        #endregion

        #region helpers

        /// <summary>
        /// Indicates whether the template supports custom entities. At
        /// the time of writing this means a PageType.CustomEntityDetails 
        /// template but technically we could support other custom entity
        /// templates types in the future so this is just a short hand for
        /// checking the CustomEntityDefinitionCode exists
        /// </summary>
        public bool IsCustomEntityTemplate()
        {
            return !string.IsNullOrWhiteSpace(CustomEntityDefinitionCode);
        }

        #endregion


    }
}
