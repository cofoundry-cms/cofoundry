namespace Cofoundry.Web;

/// <summary>
/// Data model representing a single line of text, without formatting
/// </summary>
public class SingleLineTextDataModel : IPageBlockTypeDataModel
{
    [Required]
    [Display(Name = "Text", Description = "Normally just text but basic HTML is accepted.")]
    //[Searchable]
    public string Text { get; set; }

}
