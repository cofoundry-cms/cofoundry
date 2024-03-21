﻿using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeRangeQueryBuilder
    : IAdvancedContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeRangeQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly IEnumerable<string> _customEntityDefinitionCodes;

    public ContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeRangeQueryBuilder(
        IExtendableContentRepository contentRepository,
        IEnumerable<string> customEntityDefinitionCodes
        )
    {
        ExtendableContentRepository = contentRepository;
        _customEntityDefinitionCodes = customEntityDefinitionCodes;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IReadOnlyDictionary<string, CustomEntityDataModelSchema>> AsDetails()
    {
        var query = new GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery(_customEntityDefinitionCodes);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
