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

namespace Cofoundry.Domain
{
    public class DuplicateCustomEntityCommandHandler
        : IAsyncCommandHandler<DuplicateCustomEntityCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityStoredProcedures _customEntityStoredProcedures;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly ICustomEntityDataModelMapper _customEntityDataModelMapper;

        public DuplicateCustomEntityCommandHandler(
            ICommandExecutor commandExecutor,
            CofoundryDbContext dbContext,
            ICustomEntityStoredProcedures customEntityStoredProcedures,
            ITransactionScopeManager transactionScopeFactory,
            ICustomEntityDataModelMapper customEntityDataModelMapper
            )
        {
            _commandExecutor = commandExecutor;
            _dbContext = dbContext;
            _customEntityStoredProcedures = customEntityStoredProcedures;
            _transactionScopeFactory = transactionScopeFactory;
            _customEntityDataModelMapper = customEntityDataModelMapper;
        }

        public async Task ExecuteAsync(DuplicateCustomEntityCommand command, IExecutionContext executionContext)
        {
            var customEntityToDuplicate = await GetCustomEntityToDuplicateAsync(command);
            var addCustomEntityCommand = MapCommand(command, customEntityToDuplicate);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                // Note: the underlying AddCustomEntityCommand will enforce permissions
                await _commandExecutor.ExecuteAsync(addCustomEntityCommand, executionContext);

                await _customEntityStoredProcedures.CopyBlocksToDraftAsync(
                    addCustomEntityCommand.OutputCustomEntityId,
                    customEntityToDuplicate.CustomEntityVersionId);

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

        private AddCustomEntityCommand MapCommand(DuplicateCustomEntityCommand command, CustomEntityVersion customEntityVersionToDuplicate)
        {
            EntityNotFoundException.ThrowIfNull(customEntityVersionToDuplicate, command.CustomEntityToDuplicateId);

            var addPageCommand = new AddCustomEntityCommand();
            addPageCommand.Title = command.Title;
            addPageCommand.LocaleId = command.LocaleId;
            addPageCommand.UrlSlug = command.UrlSlug;
            addPageCommand.CustomEntityDefinitionCode = customEntityVersionToDuplicate.CustomEntity.CustomEntityDefinitionCode;
            addPageCommand.Model = _customEntityDataModelMapper.Map(addPageCommand.CustomEntityDefinitionCode, customEntityVersionToDuplicate.SerializedData);

            return addPageCommand;
        }
    }
}
