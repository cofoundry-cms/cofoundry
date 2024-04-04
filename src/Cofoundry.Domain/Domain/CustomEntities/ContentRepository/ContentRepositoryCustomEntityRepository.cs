﻿using Cofoundry.Domain.Extendable;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCustomEntityRepository
        : IContentRepositoryCustomEntityRepository
        , IExtendableContentRepositoryPart
{
    private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

    public ContentRepositoryCustomEntityRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
        _customEntityDefinitionRepository = contentRepository.ServiceProvider.GetRequiredService<ICustomEntityDefinitionRepository>();
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IContentRepositoryCustomEntityByDefinitionQueryBuilder GetByDefinitionCode(string customEntityDefinitionCode)
    {
        return new ContentRepositoryCustomEntityByDefinitionQueryBuilder(ExtendableContentRepository, customEntityDefinitionCode);
    }

    public IContentRepositoryCustomEntityByDefinitionQueryBuilder GetByDefinition<TDefinition>() where TDefinition : ICustomEntityDefinition
    {
        var customEntityDefinition = _customEntityDefinitionRepository.GetRequired<TDefinition>();

        if (customEntityDefinition == null)
        {
            throw new Exception("Custom Entity Definition not returned from ICustomEntityDefinitionRepository: " + typeof(TDefinition).FullName);
        }

        return new ContentRepositoryCustomEntityByDefinitionQueryBuilder(ExtendableContentRepository, customEntityDefinition.CustomEntityDefinitionCode);
    }

    public IContentRepositoryCustomEntityByIdQueryBuilder GetById(int customEntityId)
    {
        return new ContentRepositoryCustomEntityByIdQueryBuilder(ExtendableContentRepository, customEntityId);
    }

    public IContentRepositoryCustomEntityByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> customEntityIds)
    {
        return new ContentRepositoryCustomEntityByIdRangeQueryBuilder(ExtendableContentRepository, customEntityIds);
    }

    public IContentRepositoryCustomEntityByUrlSlugQueryBuilder GetByUrlSlug(string customEntityDefinitionCode, string urlSlug)
    {
        var customEntityDefinition = _customEntityDefinitionRepository.GetRequiredByCode(customEntityDefinitionCode);

        return new ContentRepositoryCustomEntityByUrlSlugQueryBuilder(ExtendableContentRepository, customEntityDefinition, urlSlug);
    }

    public IContentRepositoryCustomEntityByUrlSlugQueryBuilder GetByUrlSlug<TDefinition>(string urlSlug) where TDefinition : ICustomEntityDefinition
    {
        var customEntityDefinition = _customEntityDefinitionRepository.GetRequired<TDefinition>();

        if (customEntityDefinition == null)
        {
            throw new Exception("Custom Entity Definition not returned from ICustomEntityDefinitionRepository: " + typeof(TDefinition).FullName);
        }

        return new ContentRepositoryCustomEntityByUrlSlugQueryBuilder(ExtendableContentRepository, customEntityDefinition, urlSlug);
    }

    public IContentRepositoryCustomEntitySearchQueryBuilder Search()
    {
        return new ContentRepositoryCustomEntitySearchQueryBuilder(ExtendableContentRepository);
    }

    /// <summary>
    /// Custom entity definitions are used to define the identity and
    /// behavior of a custom entity type. This includes meta data such
    /// as the name and description, but also the configuration of
    /// features such as whether the identity can contain a locale
    /// and whether versioning (i.e. auto-publish) is enabled.
    /// </summary>
    public IContentRepositoryCustomEntityDefinitionsRepository Definitions()
    {
        return new ContentRepositoryCustomEntityDefinitionsRepository(ExtendableContentRepository);
    }
}
