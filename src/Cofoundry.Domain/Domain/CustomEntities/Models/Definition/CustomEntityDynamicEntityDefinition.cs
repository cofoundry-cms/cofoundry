﻿namespace Cofoundry.Domain;

/// <summary>
/// IEntityDefinition for custom entities. These are dynamically generated from the custom entitity
/// definition classes.
/// </summary>
public class CustomEntityDynamicEntityDefinition : IDependableEntityDefinition
{
    public CustomEntityDynamicEntityDefinition()
    {
    }

    public CustomEntityDynamicEntityDefinition(ICustomEntityDefinition customEntityDefinition)
    {
        ArgumentNullException.ThrowIfNull(customEntityDefinition);

        EntityDefinitionCode = customEntityDefinition.CustomEntityDefinitionCode;
        Name = customEntityDefinition.Name;
    }

    public string EntityDefinitionCode { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public IQuery<IReadOnlyDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
    {
        return new GetCustomEntityEntityMicroSummariesByIdRangeQuery(ids);
    }
}
