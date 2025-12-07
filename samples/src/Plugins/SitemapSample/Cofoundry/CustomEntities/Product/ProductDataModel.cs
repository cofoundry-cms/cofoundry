using System.ComponentModel.DataAnnotations;

namespace SitemapSample;

public class ProductDataModel : ICustomEntityDataModel
{
    [MaxLength(500)]
    [Display(Description = "A short description of the product.")]
    [MultiLineText]
    public string? ShortDescription { get; set; }
}
