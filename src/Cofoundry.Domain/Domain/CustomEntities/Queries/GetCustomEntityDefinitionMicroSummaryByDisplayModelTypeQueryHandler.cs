using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.Reflection;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Query to get a custom entity definition by display model type definition.
    /// The returned object is a lightweight projection of the data defined in a custom entity 
    /// definition class and is typically used as part of another domain model.
    /// </summary>
    public class GetCustomEntityDefinitionMicroSummaryByDisplayModelTypeQueryHandler 
        : IQueryHandler<GetCustomEntityDefinitionMicroSummaryByDisplayModelTypeQuery, CustomEntityDefinitionMicroSummary>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICustomEntityDefinitionMicroSummaryMapper _customEntityDefinitionMicroSummaryMapper;

        public GetCustomEntityDefinitionMicroSummaryByDisplayModelTypeQueryHandler(
            IQueryExecutor queryExecutor,
            ICustomEntityDefinitionMicroSummaryMapper customEntityDefinitionMicroSummaryMapper
            )
        {
            _queryExecutor = queryExecutor;
            _customEntityDefinitionMicroSummaryMapper = customEntityDefinitionMicroSummaryMapper;
        }

        #endregion

        public async Task<CustomEntityDefinitionMicroSummary> ExecuteAsync(GetCustomEntityDefinitionMicroSummaryByDisplayModelTypeQuery query, IExecutionContext executionContext)
        {
            var dataModelType = query.DisplayModelType
                .GetInterfaces()
                .Select(t => t.GetTypeInfo())
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICustomEntityDisplayModel<>))
                .Select(i => i.GetGenericArguments().Single())
                .SingleOrDefault();

            if (dataModelType == null)
            {
                throw new ArgumentException("query.DisplayModelType is not of type ICustomEntityDisplayModel<>");
            }

            var allDefinitions = await _queryExecutor.ExecuteAsync(new GetAllCustomEntityDefinitionSummariesQuery(), executionContext);

            var definitions = allDefinitions
                .Where(d => d.DataModelType == dataModelType)
                .ToList();

            if (!definitions.Any())
            {
                return null;
            }
            else if (definitions.Count > 1)
            {
                throw new Exception($"{nameof(ICustomEntityDataModel)} implementations cannot be used on multiple custom entity definitions.");
            }

            var microSummary = _customEntityDefinitionMicroSummaryMapper.Map(definitions.First());

            return microSummary;
        }
    }
}
