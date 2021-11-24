namespace Cofoundry.Domain.Tests.Integration.SeedData
{
    public class TestGenericPageInfo
    {
        public TestGenericPageInfo(TestDirectoryInfo directory)
        {
            FullUrlPath = directory.FullPath + "/" + UrlPath;
        }

        public int PageId { get; set; }

        public string UrlPath = "test-page";

        public string Title = "Test Page with Generic Template";

        public string FullUrlPath { get; private set; }
    }
}
