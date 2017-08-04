using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetPageBlockTypeDetailsByIdQueryHandler
        : IAsyncQueryHandler<GetByIdQuery<PageBlockTypeDetails>, PageBlockTypeDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<PageBlockTypeDetails>, PageBlockTypeDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IEnumerable<IPageBlockTypeDataModel> _allPageBlockTypeDataModels;
        private readonly DynamicDataModelSchemaMapper _dynamicDataModelTypeMapper;

        public GetPageBlockTypeDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IEnumerable<IPageBlockTypeDataModel> allPageBlockTypeDataModels,
            DynamicDataModelSchemaMapper dynamicDataModelTypeMapper
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _allPageBlockTypeDataModels = allPageBlockTypeDataModels;
            _dynamicDataModelTypeMapper = dynamicDataModelTypeMapper;
        }

        #endregion

        #region execution

        public async Task<PageBlockTypeDetails> ExecuteAsync(GetByIdQuery<PageBlockTypeDetails> query, IExecutionContext executionContext)
        {
            var result = await GetPageBlockTypeById(query.Id);
            if (result == null) return null;

            var dataModelType = GetPageBlockDataModelType(result);

            _dynamicDataModelTypeMapper.Map(result, dataModelType);

            return result;
        }

        private async Task<PageBlockTypeDetails> GetPageBlockTypeById(int id)
        {
            var allBlockTypes = await _queryExecutor.GetAllAsync<PageBlockTypeSummary>();

            var blockTypeTypeSummary = allBlockTypes
                .SingleOrDefault(t => t.PageBlockTypeId == id);

            return Mapper.Map<PageBlockTypeDetails>(blockTypeTypeSummary);
        }

        private Type GetPageBlockDataModelType(PageBlockTypeDetails blockTypeDetails)
        {
            var dataModelName = blockTypeDetails.FileName + "DataModel";

            var dataModel = _allPageBlockTypeDataModels
                .Select(m => m.GetType())
                .Where(m => m.Name == dataModelName)
                .SingleOrDefault();

            var blockType = Mapper.Map<PageBlockTypeDetails>(blockTypeDetails);

            EntityNotFoundException.ThrowIfNull(dataModel, dataModelName);

            return dataModel;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<PageBlockTypeDetails> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
