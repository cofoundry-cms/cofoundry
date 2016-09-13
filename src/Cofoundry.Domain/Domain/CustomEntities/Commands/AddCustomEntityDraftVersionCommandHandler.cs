using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core.EntityFramework;
using System.Data.SqlClient;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class AddCustomEntityDraftVersionCommandHandler 
        : IAsyncCommandHandler<AddCustomEntityDraftVersionCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IEntityFrameworkSqlExecutor _entityFrameworkSqlExecutor;

        public AddCustomEntityDraftVersionCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            ICustomEntityCache customEntityCache,
            IMessageAggregator messageAggregator,
            IPermissionValidationService permissionValidationService,
            IEntityFrameworkSqlExecutor entityFrameworkSqlExecutor
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _customEntityCache = customEntityCache;
            _messageAggregator = messageAggregator;
            _permissionValidationService = permissionValidationService;
            _entityFrameworkSqlExecutor = entityFrameworkSqlExecutor;
        }

        #endregion

        #region execution
        
        public async Task ExecuteAsync(AddCustomEntityDraftVersionCommand command, IExecutionContext executionContext)
        {
            var definitionCode = await QueryVersionAndGetDefinitionCode(command).FirstOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(definitionCode, command.CustomEntityId);

            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(definitionCode);

            var newVersionId = await _entityFrameworkSqlExecutor
                .ExecuteCommandWithOutputAsync<int?>("Cofoundry.CustomEntity_AddDraft",
                "CustomEntityVersionId",
                 new SqlParameter("CustomEntityId", command.CustomEntityId),
                 new SqlParameter("CopyFromCustomEntityVersionId", command.CopyFromCustomEntityVersionId),
                 new SqlParameter("CreateDate", executionContext.ExecutionDate),
                 new SqlParameter("CreatorId", executionContext.UserContext.UserId)
                 );

            if (!newVersionId.HasValue)
            {
                throw new UnexpectedSqlStoredProcedureResultException("Cofoundry.CustomEntity_AddDraft", "No CustomEntityVersionId was returned.");
            }

            command.OutputCustomEntityVersionId = newVersionId.Value;
            _customEntityCache.Clear(definitionCode, command.CustomEntityId);

            await _messageAggregator.PublishAsync(new CustomEntityDraftVersionAddedMessage()
            {
                CustomEntityId = command.CustomEntityId,
                CustomEntityVersionId = newVersionId.Value,
                CustomEntityDefinitionCode = definitionCode
            });
        }

        #endregion

        #region helpers

        private IQueryable<string> QueryVersionAndGetDefinitionCode(AddCustomEntityDraftVersionCommand command)
        {
            var dbQuery = _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .Where(v => v.CustomEntityId == command.CustomEntityId);

            if (command.CopyFromCustomEntityVersionId.HasValue)
            {
                dbQuery = dbQuery.Where(v => v.CustomEntityVersionId == command.CopyFromCustomEntityVersionId);
            }
            else
            {
                dbQuery = dbQuery
                    .Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published || v.WorkFlowStatusId == (int)WorkFlowStatus.Approved)
                    .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
            }

            return dbQuery
                .Select(v => v.CustomEntity.CustomEntityDefinitionCode);
        }

        #endregion
    }
}
