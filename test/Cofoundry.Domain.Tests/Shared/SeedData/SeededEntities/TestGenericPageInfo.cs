namespace Cofoundry.Domain.Tests.SeedData;

public class TestCustomEntityPageInfo
{
    private readonly TestDirectoryInfo _directory;

    public TestCustomEntityPageInfo(TestDirectoryInfo directory)
    {
        _directory = directory;
    }

    public int PageId { get; set; }

    public string Title = "Test Page with Custom Entity Template";

    public IdCustomEntityRoutingRule RoutingRule = new();

    public string GetFullPath(int customEntityId)
    {
        return _directory.FullPath + "/" + customEntityId;
    }
}
