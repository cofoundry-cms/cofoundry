using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Creates a new custom entity, copying from an existing custom entity.
    /// </summary>
    public class DuplicateCustomEntityCommandHandler
        : ICommandHandler<DuplicateCustomEntityCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityStoredProcedures _customEntityStoredProcedures;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly ICustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public DuplicateCustomEntityCommandHandler(
            ICommandExecutor commandExecutor,
            CofoundryDbContext dbContext,
            ICustomEntityStoredProcedures customEntityStoredProcedures,
            ITransactionScopeManager transactionScopeFactory,
            ICustomEntityDataModelMapper customEntityDataModelMapper,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _commandExecutor = commandExecutor;
            _dbContext = dbContext;
            _customEntityStoredProcedures = customEntityStoredProcedures;
            _transactionScopeFactory = transactionScopeFactory;
            _customEntityDataModelMapper = customEntityDataModelMapper;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        public async Task ExecuteAsync(DuplicateCustomEntityCommand command, IExecutionContext executionContext)
        {
            var customEntityToDuplicate = await GetCustomEntityToDuplicateAsync(command);
            var definition = _customEntityDefinitionRepository.GetByCode(customEntityToDuplicate.CustomEntity.CustomEntityDefinitionCode);
            var addCustomEntityCommand = MapCommand(command, customEntityToDuplicate);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                // Note: the underlying AddCustomEntityCommand will enforce permissions
                await _commandExecutor.ExecuteAsync(addCustomEntityCommand, executionContext);

                await _customEntityStoredProcedures.CopyBlocksToDraftAsync(
                    addCustomEntityCommand.OutputCustomEntityId,
                    customEntityToDuplicate.CustomEntityVersionId);

                if (definition.AutoPublish)
                {
                    var publishCommand = new PublishCustomEntityCommand(addCustomEntityCommand.OutputCustomEntityId);
                    await _commandExecutor.ExecuteAsync(publishCommand);
                }

                await scope.CompleteAsync();
            }

            // Set Ouput
            command.OutputCustomEntityId = addCustomEntityCommand.OutputCustomEntityId;
        }

        private Task<CustomEntityVersion> GetCustomEntityToDuplicateAsync(DuplicateCustomEntityCommand command)
        {
            return _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .Include(e => e.CustomEntity)
                .FilterActive()
                .FilterByCustomEntityId(command.CustomEntityToDuplicateId)
                .OrderByLatest()
                .FirstOrDefaultAsync();
        }

        private AddCustomEntityCommand MapCommand(
            DuplicateCustomEntityCommand command, 
            CustomEntityVersion customEntityVersionToDuplicate
            )
        {
            EntityNotFoundException.ThrowIfNull(customEntityVersionToDuplicate, command.CustomEntityToDuplicateId);

            var addCustomEntityCommand = new AddCustomEntityCommand();
            addCustomEntityCommand.Title = command.Title;
            addCustomEntityCommand.LocaleId = command.LocaleId;
            addCustomEntityCommand.UrlSlug = command.UrlSlug;
            addCustomEntityCommand.CustomEntityDefinitionCode = customEntityVersionToDuplicate.CustomEntity.CustomEntityDefinitionCode;
            addCustomEntityCommand.Model = _customEntityDataModelMapper.Map(addCustomEntityCommand.CustomEntityDefinitionCode, customEntityVersionToDuplicate.SerializedData);
            
            return addCustomEntityCommand;
        }
    }
}
