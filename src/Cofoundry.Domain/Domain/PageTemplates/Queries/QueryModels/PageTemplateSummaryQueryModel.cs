using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.QueryModels;

public class PageTemplateSummaryQueryModel
{
    public PageTemplate PageTemplate { get; set; }

    public int NumPages { get; set; }

    public int NumRegions { get; set; }
}
