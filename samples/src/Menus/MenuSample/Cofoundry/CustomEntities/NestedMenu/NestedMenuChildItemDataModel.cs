using System.ComponentModel.DataAnnotations;

namespace MenuSample;

/// <summary>
/// The interface INestedDataModel tells Cofoundry that
/// is can be nested inside another data model. 
public class NestedMenuChildItemDataModel : INestedDataModel
{
    [Required]
    [MaxLength(30)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Page]
    public int PageId { get; set; }
}
