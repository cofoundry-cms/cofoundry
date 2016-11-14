using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using System.Data.Entity;
using AutoMapper;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetPageModuleTypeDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<PageModuleTypeDetails>, PageModuleTypeDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<PageModuleTypeDetails>, PageModuleTypeDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageModuleDataModel[] _allModuleDataModels;
        private readonly DynamicDataModelSchemaMapper _dynamicDataModelTypeMapper;

        public GetPageModuleTypeDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageModuleDataModel[] allModuleDataModels,
            DynamicDataModelSchemaMapper dynamicDataModelTypeMapper
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _allModuleDataModels = allModuleDataModels;
            _dynamicDataModelTypeMapper = dynamicDataModelTypeMapper;
        }

        #endregion

        public async Task<PageModuleTypeDetails> ExecuteAsync(GetByIdQuery<PageModuleTypeDetails> query, IExecutionContext executionContext)
        {
            var result = await GetPageModuleTypeById(query.Id);
            if (result == null) return null;

            var dataModelType = GetModuleDataModelType(result);

            _dynamicDataModelTypeMapper.Map(result, dataModelType);

            return result;
        }

        private async Task<PageModuleTypeDetails> GetPageModuleTypeById(int id)
        {
            var allModuleTypes = await _queryExecutor.GetAllAsync<PageModuleTypeSummary>();

            var moduleTypeSummary = allModuleTypes
                .SingleOrDefault(t => t.PageModuleTypeId == id);

            return Mapper.Map<PageModuleTypeDetails>(moduleTypeSummary);
        }

        private Type GetModuleDataModelType(PageModuleTypeDetails moduleTypeDetails)
        {
            var dataModelName = moduleTypeDetails.FileName + "DataModel";

            var dataModel = _allModuleDataModels
                .Select(m => m.GetType())
                .Where(m => m.Name == dataModelName)
                .SingleOrDefault();

            var moduleType = Mapper.Map<PageModuleTypeDetails>(moduleTypeDetails);

            EntityNotFoundException.ThrowIfNull(dataModel, dataModelName);

            return dataModel;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<PageModuleTypeDetails> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
