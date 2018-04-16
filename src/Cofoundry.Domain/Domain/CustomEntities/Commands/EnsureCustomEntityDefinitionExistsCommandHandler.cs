using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class EnsureCustomEntityDefinitionExistsCommandHandler 
        : IAsyncCommandHandler<EnsureCustomEntityDefinitionExistsCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly ICommandExecutor _commandExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public EnsureCustomEntityDefinitionExistsCommandHandler(
            ICommandExecutor commandExecutor,
            CofoundryDbContext dbContext,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _commandExecutor = commandExecutor;
            _dbContext = dbContext;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region Execute

        public async Task ExecuteAsync(EnsureCustomEntityDefinitionExistsCommand command, IExecutionContext executionContext)
        {
            var customEntityDefinition = _customEntityDefinitionRepository.GetByCode(command.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(customEntityDefinition, command.CustomEntityDefinitionCode);

            var dbDefinition = await _dbContext
                .CustomEntityDefinitions
                .FilterByCode(command.CustomEntityDefinitionCode)
                .SingleOrDefaultAsync();

            if (dbDefinition == null)
            {
                await _commandExecutor.ExecuteAsync(new EnsureEntityDefinitionExistsCommand(command.CustomEntityDefinitionCode), executionContext);

                dbDefinition = new CustomEntityDefinition()
                {
                    CustomEntityDefinitionCode = customEntityDefinition.CustomEntityDefinitionCode,
                    ForceUrlSlugUniqueness = customEntityDefinition.ForceUrlSlugUniqueness,
                    HasLocale = customEntityDefinition.HasLocale
                };

                if (customEntityDefinition is IOrderableCustomEntityDefinition)
                {
                    dbDefinition.IsOrderable = ((IOrderableCustomEntityDefinition)customEntityDefinition).Ordering != CustomEntityOrdering.None;
                }

                _dbContext.CustomEntityDefinitions.Add(dbDefinition);
                await _dbContext.SaveChangesAsync();
            }
        }

        #endregion
    }
}
