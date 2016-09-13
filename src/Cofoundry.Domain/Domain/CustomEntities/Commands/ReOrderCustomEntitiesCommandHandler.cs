using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System.Data.Entity;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.EntityFramework;
using System.Data.SqlClient;
using Cofoundry.Core;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    public class ReOrderCustomEntitiesCommandHandler 
        : IAsyncCommandHandler<ReOrderCustomEntitiesCommand>
        , IPermissionRestrictedCommandHandler<ReOrderCustomEntitiesCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityFrameworkSqlExecutor _sqlExecutor;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ICustomEntityCodeDefinitionRepository _customEntityDefinitionRepository;

        public ReOrderCustomEntitiesCommandHandler(
            CofoundryDbContext dbContext,
            IEntityFrameworkSqlExecutor sqlExecutor,
            ICustomEntityCache customEntityCache,
            IMessageAggregator messageAggregator,
            ICustomEntityCodeDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _sqlExecutor = sqlExecutor;
            _customEntityCache = customEntityCache;
            _messageAggregator = messageAggregator;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(ReOrderCustomEntitiesCommand command, IExecutionContext executionContext)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(command.CustomEntityDefinitionCode) as IOrderableCustomEntityDefinition;

            if (definition == null || definition.Ordering == CustomEntityOrdering.None)
            {
                throw new InvalidOperationException("Cannot re-order a custom entity type with a definition that does not implement IOrderableCustomEntityDefinition (" + definition.GetType().Name + ")");
            }

            if (!definition.HasLocale && command.LocaleId.HasValue)
            {
                throw new ValidationException("Cannot order by locale because this custom entity type is not permitted to have a locale (" + definition.GetType().FullName + ")");
            }

            var updatedIdString = await _sqlExecutor
                .ExecuteCommandWithOutputAsync<string>("Cofoundry.CustomEntity_ReOrder", "UpdatedIds",
                    new SqlParameter("CustomEntityDefinitionCode", command.CustomEntityDefinitionCode),
                    new SqlParameter("CustomEntityIds", string.Join(",", command.OrderedCustomEntityIds)),
                    CreateNullableIntParameter("LocaleId", command.LocaleId)
                    );

            var affectedIds = IntParser.ParseFromDelimitedString(updatedIdString);

            foreach (var affectedId in affectedIds)
            {
                _customEntityCache.Clear(command.CustomEntityDefinitionCode, affectedId);
            }

            var messages = affectedIds.Select(i => new CustomEntityOrderingUpdatedMessage() {
                CustomEntityDefinitionCode = command.CustomEntityDefinitionCode,
                CustomEntityId = i
            });

            await _messageAggregator.PublishBatchAsync(messages);
        }

        private SqlParameter CreateNullableIntParameter(string name, int? value)
        {
            var parameter =  new SqlParameter("LocaleId", System.Data.SqlDbType.Int);
            if (value.HasValue)
            {
                parameter.Value = value.Value;
            }

            return parameter;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(ReOrderCustomEntitiesCommand command)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(command.CustomEntityDefinitionCode);
            yield return new CustomEntityUpdatePermission(definition);
        }

        #endregion
    }
}
