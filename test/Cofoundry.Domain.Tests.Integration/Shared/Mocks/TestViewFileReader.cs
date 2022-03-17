using Cofoundry.Core.ResourceFiles;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Tests.Integration.Mocks;

public class TestViewFileReader : IViewFileReader
{
    public const string MOCK_DATA_POSTFIX = ".MOCK.";

    private readonly ViewFileReader _viewFileReader;

    public TestViewFileReader(
        IResourceLocator resourceLocator
        )
    {
        _viewFileReader = new ViewFileReader(resourceLocator);
    }

    public Task<string> ReadViewFileAsync(string path)
    {
        // Remove any mock data fromt he path that has been appended to get around a uniqueness constraint
        var index = path.IndexOf(MOCK_DATA_POSTFIX);

        if (index > -1)
        {
            path = path.Remove(index);
        }

        return _viewFileReader.ReadViewFileAsync(path);
    }
}
