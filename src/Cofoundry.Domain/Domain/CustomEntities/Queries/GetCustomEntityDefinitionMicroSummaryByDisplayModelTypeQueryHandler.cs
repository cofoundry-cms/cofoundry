using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.Reflection;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDefinitionMicroSummaryByDisplayModelTypeQueryHandler 
        : IAsyncQueryHandler<GetCustomEntityDefinitionMicroSummaryByDisplayModelTypeQuery, CustomEntityDefinitionMicroSummary>
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

            var allDefinitions = await _queryExecutor.GetAllAsync<CustomEntityDefinitionSummary>(executionContext);

            var definition = allDefinitions.FirstOrDefault(d => d.DataModelType == dataModelType);

            var microSummary = _customEntityDefinitionMicroSummaryMapper.Map(definition);

            return microSummary;
        }
    }
}
