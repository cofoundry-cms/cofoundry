using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.QueryModels;

[Obsolete("The page grouping system will be revised in an upcomming release.")]
public class PageGroupSummaryQueryModel
{
    public required PageGroup PageGroup { get; set; }

    public required User Creator { get; set; }

    public int NumPages { get; set; }
}
