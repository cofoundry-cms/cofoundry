using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Web;

/// <summary>
/// Data model representing a list of text items 
/// </summary>
public class TextListDataModel : IPageBlockTypeDataModel
{
    [Display(Name = "Title")]
    public string? Title { get; set; }

    [Required, Display(Name = "Text list")]
    public string TextList { get; set; } = string.Empty;

    [Display(Name = "Numbered?")]
    public bool IsNumbered { get; set; }
}
