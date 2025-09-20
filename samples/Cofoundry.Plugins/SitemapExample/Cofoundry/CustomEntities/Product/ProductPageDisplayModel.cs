namespace SitemapExample;

public class ProductPageDisplayModel : ICustomEntityPageDisplayModel<ProductDataModel>
{
    public string PageTitle { get; set; } = string.Empty;

    public string? MetaDescription { get; set; }

    public string? ShortDescription { get; set; }
}
