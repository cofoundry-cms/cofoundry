using System.ComponentModel.DataAnnotations;

namespace Cofoundry.BasicTestSite;

public class CarouselDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
{
    [MaxLength(100)]
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    [NestedDataModelCollection(IsOrderable = true, MinItems = 2, MaxItems = 6)]
    public IReadOnlyCollection<CarouselItemDataModel> Items { get; set; } = Array.Empty<CarouselItemDataModel>();
}
