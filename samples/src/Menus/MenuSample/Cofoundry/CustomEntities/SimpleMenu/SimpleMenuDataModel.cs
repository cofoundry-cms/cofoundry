using System.ComponentModel.DataAnnotations;

namespace MenuSample;

/// <summary>
/// The simple menu demonstrates how you can build a content
/// manageable list of pages using the [PageCollection] attribute.
/// </summary>
public class SimpleMenuDataModel : ICustomEntityDataModel
{
    [Required]
    [PageCollection(IsOrderable = true)]
    public IReadOnlyCollection<int> PageIds { get; set; } = Array.Empty<int>();
}

