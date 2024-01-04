using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.QueryModels;

public class PageTemplateDetailsQueryModel
{
    public required PageTemplate PageTemplate { get; set; }

    public int NumPages { get; set; }

    public CustomEntityDefinitionMicroSummary? CustomEntityDefinition { get; set; }
}
