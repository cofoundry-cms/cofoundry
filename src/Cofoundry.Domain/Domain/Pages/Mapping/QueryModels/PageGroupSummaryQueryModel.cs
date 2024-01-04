using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.QueryModels;

public class PageGroupSummaryQueryModel
{
    public required PageGroup PageGroup { get; set; }

    public required User Creator { get; set; }

    public int NumPages { get; set; }
}
