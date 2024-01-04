﻿namespace Cofoundry.Domain;

public class GetCustomEntityEntityMicroSummariesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, RootEntityMicroSummary>>
{
    public GetCustomEntityEntityMicroSummariesByIdRangeQuery()
    {
    }

    public GetCustomEntityEntityMicroSummariesByIdRangeQuery(
        IEnumerable<int>? customEntityIds
        )
        : this(customEntityIds?.ToArray() ?? [])
    {
    }

    public GetCustomEntityEntityMicroSummariesByIdRangeQuery(
        IReadOnlyCollection<int> customEntityIds
        )
    {
        ArgumentNullException.ThrowIfNull(customEntityIds);

        CustomEntityIds = customEntityIds;
    }

    [Required]
    public IReadOnlyCollection<int> CustomEntityIds { get; set; } = Array.Empty<int>();
}
