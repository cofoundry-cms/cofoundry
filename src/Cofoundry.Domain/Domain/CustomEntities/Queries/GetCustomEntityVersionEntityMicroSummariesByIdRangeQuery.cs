﻿namespace Cofoundry.Domain;

public class GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, RootEntityMicroSummary>>
{
    public GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery()
    {
    }

    public GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery(
        IEnumerable<int>? ids
        )
        : this(ids?.ToArray() ?? [])
    {
    }

    public GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery(
        IReadOnlyCollection<int> ids
        )
    {
        ArgumentNullException.ThrowIfNull(ids);

        CustomEntityVersionIds = ids;
    }

    [Required]
    public IReadOnlyCollection<int> CustomEntityVersionIds { get; set; } = Array.Empty<int>();
}
