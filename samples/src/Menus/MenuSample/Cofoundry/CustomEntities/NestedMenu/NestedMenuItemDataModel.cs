using System.ComponentModel.DataAnnotations;

namespace MenuSample;

public class NestedMenuItemDataModel : INestedDataModel
{
    [MaxLength(30)]
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Page]
    public int PageId { get; set; }

    [Display(Name = "Level 2 Items")]
    [NestedDataModelCollection]
    public ICollection<NestedMenuChildItemDataModel> ChildItems { get; set; } = Array.Empty<NestedMenuChildItemDataModel>();
}
