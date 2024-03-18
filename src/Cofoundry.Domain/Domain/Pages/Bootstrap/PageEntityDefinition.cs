﻿namespace Cofoundry.Domain;

/// <summary>
/// Pages represent the dynamically navigable pages of your website. Each page uses a template 
/// which defines the regions of content that users can edit.
/// </summary>
/// <inheritdoc/>
public sealed class PageEntityDefinition : IDependableEntityDefinition
{
    public const string DefinitionCode = "COFPGE";

    public string EntityDefinitionCode => DefinitionCode;

    public string Name => "Page";

    public IQuery<IReadOnlyDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
    {
        return new GetPageEntityMicroSummariesByIdRangeQuery(ids);
    }
}
