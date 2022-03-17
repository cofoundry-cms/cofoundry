namespace Cofoundry.Domain.Tests.Integration.SeedData;

public class TestDirectoryInfo
{
    public int PageDirectoryId { get; set; }

    public string UrlPath = "test-directory";

    public string FullPath = "/test-directory";

    /// <summary>
    /// Page using the generic test template with the path 
    /// "/test-directory/test-page".
    /// </summary>
    public TestGenericPageInfo GenericPage => new TestGenericPageInfo(this);

    /// <summary>
    /// Page using the test custom entity template with the path 
    /// "/test-directory/{Id}".
    /// </summary>
    public TestCustomEntityPageInfo CustomEntityPage => new TestCustomEntityPageInfo(this);

}
