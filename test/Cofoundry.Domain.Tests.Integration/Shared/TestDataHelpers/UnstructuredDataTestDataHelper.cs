using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Tests.Integration;

/// <summary>
/// Used to make it easier to create or reference page 
/// directories in test fixtures.
/// </summary>
public class UnstructuredDataTestDataHelper
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SeededEntities _seededEntities;

    public UnstructuredDataTestDataHelper(
        IServiceProvider serviceProvider,
        SeededEntities seededEntities
        )
    {
        _serviceProvider = serviceProvider;
        _seededEntities = seededEntities;
    }

    /// <summary>
    /// Adds an unstructured data dependency reference with the specified
    /// entity as the relation type and <c>_seededEntities.CustomEntityForUnstructuredDataTests</c>
    /// as the root entity.
    /// </summary>
    /// <typeparam name="TRelatedEntityDefinition">The entity type of the related entity.</typeparam>
    /// <param name="relatedEntityId">The database Id of the related entity.</param>
    /// <param name="relatedEntityCascadeAction">The cascade action to use in the relation. Both cascade actions should be tested.</param>
    public async Task AddAsync<TRelatedEntityDefinition>(int relatedEntityId, RelatedEntityCascadeAction relatedEntityCascadeAction)
        where TRelatedEntityDefinition : IEntityDefinition, new()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CofoundryDbContext>();
        var contentRepository = scope
            .ServiceProvider
            .GetRequiredService<IAdvancedContentRepository>()
            .WithElevatedPermissions();

        var entityDefinition = new TRelatedEntityDefinition();
        await contentRepository.ExecuteCommandAsync(new EnsureEntityDefinitionExistsCommand(entityDefinition.EntityDefinitionCode));

        dbContext.UnstructuredDataDependencies.Add(new UnstructuredDataDependency()
        {
            RootEntityDefinitionCode = _seededEntities.CustomEntityForUnstructuredDataTests.CustomEntityDefinitionCode,
            RootEntityId = _seededEntities.CustomEntityForUnstructuredDataTests.CustomEntityId,
            RelatedEntityDefinitionCode = entityDefinition.EntityDefinitionCode,
            RelatedEntityId = relatedEntityId,
            RelatedEntityCascadeActionId = (int)relatedEntityCascadeAction
        });

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Adds an unstructured data dependency reference between
    /// the specified entities
    /// </summary>
    /// <param name="entityCustomEntityDefinitionCode">The entity type of the root entity</param>
    /// <param name="entityId">The database Id of the root entity.</param>
    /// <param name="relatedEntityCustomEntityDefinitionCode">The entity type of the related entity.</param>
    /// <param name="relatedEntityId">The database Id of the related entity.</param>
    /// <param name="relatedEntityCascadeAction">The cascade action to use in the relation. Both cascade actions should be tested.</param>
    public async Task AddAsync(
        string entityCustomEntityDefinitionCode,
        int entityId,
        string relatedEntityCustomEntityDefinitionCode,
        int relatedEntityId,
        RelatedEntityCascadeAction relatedEntityCascadeAction
        )
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CofoundryDbContext>();
        var contentRepository = scope
            .ServiceProvider
            .GetRequiredService<IAdvancedContentRepository>()
            .WithElevatedPermissions();

        await contentRepository.ExecuteCommandAsync(new EnsureEntityDefinitionExistsCommand(entityCustomEntityDefinitionCode));
        await contentRepository.ExecuteCommandAsync(new EnsureEntityDefinitionExistsCommand(relatedEntityCustomEntityDefinitionCode));

        dbContext.UnstructuredDataDependencies.Add(new UnstructuredDataDependency()
        {
            RootEntityDefinitionCode = entityCustomEntityDefinitionCode,
            RootEntityId = entityId,
            RelatedEntityDefinitionCode = relatedEntityCustomEntityDefinitionCode,
            RelatedEntityId = relatedEntityId,
            RelatedEntityCascadeActionId = (int)relatedEntityCascadeAction
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task<UnstructuredDataDependency> GetAsync<TEntityDefinition>(int entityId)
        where TEntityDefinition : IEntityDefinition, new()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CofoundryDbContext>();
        var entityDefinition = new TEntityDefinition();

        var relation = await dbContext
            .UnstructuredDataDependencies
            .AsNoTracking()
            .FilterByRootEntity(_seededEntities.CustomEntityForUnstructuredDataTests.CustomEntityDefinitionCode, _seededEntities.CustomEntityForUnstructuredDataTests.CustomEntityId)
            .FilterByRelatedEntity(entityDefinition.EntityDefinitionCode, entityId)
            .SingleOrDefaultAsync();

        return relation;
    }
}
