using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class DependableEntityDeleteCommandValidator : IDependableEntityDeleteCommandValidator
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IEntityDefinitionRepository _entityDefinitionRepository;

        public DependableEntityDeleteCommandValidator(
            IQueryExecutor queryExecutor,
            IEntityDefinitionRepository entityDefinitionRepository
            )
        {
            _queryExecutor = queryExecutor;
            _entityDefinitionRepository = entityDefinitionRepository;
        }

        public async Task ValidateAsync(string entityDefinitionCode, int entityId, IExecutionContext executionContext)
        {
            if (string.IsNullOrWhiteSpace(entityDefinitionCode)) throw new ArgumentEmptyException(nameof(entityDefinitionCode));
            if (entityId < 1) throw new ArgumentOutOfRangeException(nameof(entityDefinitionCode), nameof(entityId) + "must be a positive integer.");
            if (executionContext == null) throw new ArgumentEmptyException(nameof(executionContext));

            var entityDefinition = _entityDefinitionRepository.GetRequiredByCode(entityDefinitionCode);
            var requiredDependencies = await _queryExecutor.ExecuteAsync(new GetEntityDependencySummaryByRelatedEntityIdQuery()
            {
                EntityDefinitionCode = entityDefinition.EntityDefinitionCode,
                EntityId = entityId,
                ExcludeDeletable = true
            }, executionContext);

            RequiredDependencyConstaintViolationException.ThrowIfCannotDelete(entityDefinition, requiredDependencies);
        }
    }
}
