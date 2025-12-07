using System.ComponentModel.DataAnnotations;

namespace MenuSample;

public class MultiLevelMenuDataModel : ICustomEntityDataModel
{
    [Required]
    [NestedDataModelCollection(IsOrderable = true)]
    public IReadOnlyCollection<MultiLevelMenuNodeDataModel> Items { get; set; } = Array.Empty<MultiLevelMenuNodeDataModel>();
}
