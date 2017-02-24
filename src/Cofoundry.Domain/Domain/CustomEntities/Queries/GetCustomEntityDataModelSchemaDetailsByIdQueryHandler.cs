using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDataModelSchemaDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByStringQuery<CustomEntityDataModelSchema>, CustomEntityDataModelSchema>
        , IPermissionRestrictedQueryHandler<GetByStringQuery<CustomEntityDataModelSchema>, CustomEntityDataModelSchema>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly DynamicDataModelSchemaMapper _dynamicDataModelTypeMapper;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public GetCustomEntityDataModelSchemaDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            DynamicDataModelSchemaMapper dynamicDataModelTypeMapper,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _dynamicDataModelTypeMapper = dynamicDataModelTypeMapper;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<CustomEntityDataModelSchema> ExecuteAsync(GetByStringQuery<CustomEntityDataModelSchema> query, IExecutionContext executionContext)
        {
            var definition = await _queryExecutor.GetByIdAsync<CustomEntityDefinitionSummary>(query.Id);
            if (definition == null) return null;

            var result = new CustomEntityDataModelSchema();

            _dynamicDataModelTypeMapper.Map(result, definition.DataModelType);

            return result;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByStringQuery<CustomEntityDataModelSchema> query)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.Id);
            yield return new CustomEntityReadPermission(definition);
        }

        #endregion
    }
}
