using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A Page Template represents a physical view template file and is used
    /// by a Page to render out content. This projection contains minimal information
    /// for uses such as selection lists.
    /// </summary>
    public class PageTemplateMicroSummary
    {
        /// <summary>
        /// The database id of the template.
        /// </summary>
        public int PageTemplateId { get; set; }

        /// <summary>
        /// Full virtual path to the view file including the filename. This will
        /// be unique.
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Human readable display name. This is unique, so to avoid 
        /// confusion when selecting a template from a list.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If this template is of type <see cref="PageType.CustomEntityDetails"/>, this will
        /// be the type of the custom entity model used in the template e.g.
        /// if the template model is CustomEntityPageViewModel&lt;BlogPostDetailsDisplayModel&gt;
        /// this property would be BlogPostDetailsDisplayModel.
        /// </summary>
        public Type CustomEntityModelType { get; set; }

        /// <summary>
        /// If this template is of type <see cref="PageType.CustomEntityDetails"/> then 
        /// it will represent the details page of a specific custom entity
        /// definition.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Indicates if the template has been archived and is no longer available 
        /// to be used in new Pages. An archived template may still be used in an 
        /// active page to support scenarios where you need to transition between
        /// an old template to a new template.
        /// </summary>
        public bool IsArchived { get; set; }
    }
}
