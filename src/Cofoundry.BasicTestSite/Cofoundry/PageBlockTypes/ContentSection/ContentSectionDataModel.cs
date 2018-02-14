using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.BasicTestSite
{
    /// <summary>
    /// Each block type should have a data model that implements IPageModuleDataModel that 
    /// describes the data to store in the database. Data is stored in an unstructured 
    /// format (json) so simple serializable data types are best.
    /// 
    /// Attributes can be used to describe validations as well as hints to the 
    /// content editor UI on how to render the data input controls.
    /// 
    /// See https://github.com/cofoundry-cms/cofoundry/wiki/Page-Module-Types
    /// for more information
    /// </summary>
    public class ContentSectionDataModel : IPageBlockTypeDataModel
    {
        [Display(Description = "Optional title to display at the top of the section")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Text", Description = "Rich text displayed at full width")]
        [Html(HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Headings, HtmlToolbarPreset.Media)]
        public string HtmlText { get; set; }


        [CustomEntityMultiTypeCollection(
            CategoryCustomEntityDefinition.DefinitionCode,
            BlogPostCustomEntityDefinition.DefinitionCode,
            IsOrderable = true)]
        public ICollection<CustomEntityIdentity> Entities { get; set; }
    }
}