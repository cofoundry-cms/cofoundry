using System.ComponentModel.DataAnnotations;

namespace PageBlockTypeSample;

/// <summary>
/// Each block type should have a data model that implements IPageBlockTypeDataModel that 
/// describes the data to store in the database. Data is stored in an unstructured 
/// format (json) so simple serializable data types are best.
/// 
/// Attributes can be used to describe validations as well as hints to the 
/// content editor UI on how to render the data input controls.
/// 
/// See https://www.cofoundry.org/docs/content-management/page-block-types
/// for more information
/// </summary>
public class PageListDataModel : IPageBlockTypeDataModel
{
    [Display(Name = "Pages", Description = "The pages to display, orderable by drag and drop.")]
    [PageCollection(IsOrderable = true)]
    public IReadOnlyCollection<int> PageIds { get; set; } = Array.Empty<int>();
}
