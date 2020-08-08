using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Query to extract and return meta data information about a custom 
    /// entity data model for a range of custom entity definitions.
    /// </summary>
    public class GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQueryHandler
        : IQueryHandler<GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery, IDictionary<string, CustomEntityDataModelSchema>>
        , IPermissionRestrictedQueryHandler<GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery, IDictionary<string, CustomEntityDataModelSchema>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDynamicDataModelSchemaMapper _dynamicDataModelTypeMapper;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IDynamicDataModelSchemaMapper dynamicDataModelTypeMapper,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _dynamicDataModelTypeMapper = dynamicDataModelTypeMapper;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        public async Task<IDictionary<string, CustomEntityDataModelSchema>> ExecuteAsync(GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery query, IExecutionContext executionContext)
        {
            var definitionQuery = new GetAllCustomEntityDefinitionSummariesQuery();
            var definitions = await _queryExecutor.ExecuteAsync(definitionQuery, executionContext);

            var results = new Dictionary<string, CustomEntityDataModelSchema>();

            foreach (var definition in definitions
                .Where(d => query.CustomEntityDefinitionCodes.Contains(d.CustomEntityDefinitionCode)))
            {
                var result = new CustomEntityDataModelSchema();
                result.CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode;
                _dynamicDataModelTypeMapper.Map(result, definition.DataModelType);

                results.Add(definition.CustomEntityDefinitionCode, result);
            }

            return results;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery query)
        {
            foreach (var code in query.CustomEntityDefinitionCodes)
            {
                var definition = _customEntityDefinitionRepository.GetByCode(code);
                EntityNotFoundException.ThrowIfNull(definition, code);

                yield return new CustomEntityReadPermission(definition);
            }
        }

        #endregion
    }
}
