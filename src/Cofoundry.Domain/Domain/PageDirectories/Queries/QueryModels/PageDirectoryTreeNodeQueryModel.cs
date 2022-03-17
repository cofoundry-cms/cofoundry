using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.QueryModels;

public class PageDirectoryTreeNodeQueryModel
{
    public PageDirectory PageDirectory { get; set; }
    public User Creator { get; set; }
    public int NumPages { get; set; }
}
