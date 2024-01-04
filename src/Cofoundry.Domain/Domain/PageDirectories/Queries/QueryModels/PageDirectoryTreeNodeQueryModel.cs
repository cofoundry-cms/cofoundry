using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class PageDirectoryTreeNodeQueryModel
{
    public required PageDirectory PageDirectory { get; set; }
    public required User Creator { get; set; }
    public int NumPages { get; set; }
}
