namespace Cofoundry.BasicTestSite;

public class PageMetaDataDataModel : IEntityExtensionDataModel
{
    public string Title { get; set; }

    public string Description { get; set; }

    [Image]
    public int? OpenGraphImage { get; set; }
}
