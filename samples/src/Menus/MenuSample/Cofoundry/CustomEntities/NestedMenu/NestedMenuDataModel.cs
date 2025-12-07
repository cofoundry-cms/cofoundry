using System.ComponentModel.DataAnnotations;

namespace MenuSample;

/// <summary>
/// <para>
/// The nested menu demonstrates how you can build menus
/// with a pre-defined number of menu levels. This is achieved
/// using nested data models and the [NestedDataModelCollection]
/// attribute.
/// </para>
/// <para>
/// This example only contains one nested menu level,
/// but you could define more by creating and nested more menu 
/// types. To use an indeterminate number of menu levels (i.e. a tree 
/// structure) have a look at the multi-level menu example.
/// </para>
/// </summary>
public class NestedMenuDataModel : ICustomEntityDataModel
{
    [Required]
    [NestedDataModelCollection(IsOrderable = true)]
    public IReadOnlyCollection<NestedMenuItemDataModel> Items { get; set; } = Array.Empty<NestedMenuItemDataModel>();
}
