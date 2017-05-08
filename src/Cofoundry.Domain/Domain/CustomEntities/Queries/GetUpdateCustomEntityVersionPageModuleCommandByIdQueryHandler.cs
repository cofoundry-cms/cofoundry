using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class GetUpdateCustomEntityVersionPageModuleCommandByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<UpdateCustomEntityVersionPageModuleCommand>, UpdateCustomEntityVersionPageModuleCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageVersionModuleModelMapper _pageVersionModuleModelMapper;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetUpdateCustomEntityVersionPageModuleCommandByIdQueryHandler(
            CofoundryDbContext dbContext,
            IPageVersionModuleModelMapper pageVersionModuleModelMapper,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _pageVersionModuleModelMapper = pageVersionModuleModelMapper;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<UpdateCustomEntityVersionPageModuleCommand> ExecuteAsync(GetByIdQuery<UpdateCustomEntityVersionPageModuleCommand> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .CustomEntityVersionPageModules
                .AsNoTracking()
                .Where(m => m.CustomEntityVersionPageModuleId == query.Id)
                .Select(m => new
                {
                    Module = m,
                    ModuleTypeFileName = m.PageModuleType.FileName,
                    CustomEntityDefinitionCode = m.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode
                })
                .SingleOrDefaultAsync();

            if (dbResult == null) return null;
            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityReadPermission>(dbResult.CustomEntityDefinitionCode);

            var result = Map(dbResult.Module, dbResult.ModuleTypeFileName);
            return result;
        }

        #endregion

        #region private helpers

        private UpdateCustomEntityVersionPageModuleCommand Map(CustomEntityVersionPageModule dbModule, string moduleTypeFileName)
        {
            var result = Mapper.Map<UpdateCustomEntityVersionPageModuleCommand>(dbModule);
            result.DataModel = _pageVersionModuleModelMapper.MapDataModel(moduleTypeFileName, dbModule);

            return result;
        }

        #endregion

    }
}
