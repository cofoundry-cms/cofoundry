using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Query to extract and return meta data information about a custom 
    /// entity data model for a specific custom entity definition.
    /// </summary>
    public class GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQueryHandler 
        : IQueryHandler<GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery, CustomEntityDataModelSchema>
        , IPermissionRestrictedQueryHandler<GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery, CustomEntityDataModelSchema>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDynamicDataModelSchemaMapper _dynamicDataModelTypeMapper;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQueryHandler(
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

        public async Task<CustomEntityDataModelSchema> ExecuteAsync(GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            var definitionQuery = new GetCustomEntityDefinitionSummaryByCodeQuery(query.CustomEntityDefinitionCode);
            var definition = await _queryExecutor.ExecuteAsync(definitionQuery, executionContext);
            if (definition == null) return null;

            var result = new CustomEntityDataModelSchema();
            result.CustomEntityDefinitionCode = definition.CustomEntityDefinitionCode;

            _dynamicDataModelTypeMapper.Map(result, definition.DataModelType);

            return result;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery query)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            yield return new CustomEntityReadPermission(definition);
        }

        #endregion
    }
}
