using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class EnsureEntityDefinitionExistsCommandHandler
    : ICommandHandler<EnsureEntityDefinitionExistsCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IEntityDefinitionRepository _entityDefinitionRepository;

    public EnsureEntityDefinitionExistsCommandHandler(
        CofoundryDbContext dbContext,
        IEntityDefinitionRepository entityDefinitionRepository
        )
    {
        _dbContext = dbContext;
        _entityDefinitionRepository = entityDefinitionRepository;
    }

    public async Task ExecuteAsync(EnsureEntityDefinitionExistsCommand command, IExecutionContext executionContext)
    {
        var entityDefinition = _entityDefinitionRepository.GetRequiredByCode(command.EntityDefinitionCode);

        var dbDefinition = await _dbContext
            .EntityDefinitions
            .SingleOrDefaultAsync(e => e.EntityDefinitionCode == command.EntityDefinitionCode);

        if (dbDefinition == null)
        {
            dbDefinition = new EntityDefinition()
            {
                EntityDefinitionCode = entityDefinition.EntityDefinitionCode,
                Name = entityDefinition.Name
            };

            _dbContext.EntityDefinitions.Add(dbDefinition);
            await _dbContext.SaveChangesAsync();
        }
    }
}
