using System.ComponentModel.DataAnnotations;

namespace HangfireSample;

public class ProductDataModel : ICustomEntityDataModel
{
    [Required]
    public string Description { get; set; } = string.Empty;
}
