using System.ComponentModel.DataAnnotations;

namespace Cofoundry.BasicTestSite;

public class CarouselDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
{
    [MaxLength(100)]
    [Required]
    public string Title { get; set; }

    [Required]
    [NestedDataModelCollection(IsOrderable = true, MinItems = 2, MaxItems = 6)]
    public ICollection<CarouselItemDataModel> Items { get; set; }
}
