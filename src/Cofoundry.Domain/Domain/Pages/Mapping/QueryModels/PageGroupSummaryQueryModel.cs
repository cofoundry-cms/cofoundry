using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.QueryModels;

public class PageGroupSummaryQueryModel
{
    public PageGroup PageGroup { get; set; }

    public User Creator { get; set; }

    public int NumPages { get; set; }
}
