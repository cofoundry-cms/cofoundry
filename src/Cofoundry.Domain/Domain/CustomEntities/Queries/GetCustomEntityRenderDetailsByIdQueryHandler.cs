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

namespace Cofoundry.Domain
{
    public class GetCustomEntityRenderDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetCustomEntityRenderDetailsByIdQuery, CustomEntityRenderDetails>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly CustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly EntityVersionPageModuleMapper _entityVersionPageModuleMapper;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            CustomEntityDataModelMapper customEntityDataModelMapper,
            EntityVersionPageModuleMapper entityVersionPageModuleMapper,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _customEntityDataModelMapper = customEntityDataModelMapper;
            _entityVersionPageModuleMapper = entityVersionPageModuleMapper;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<CustomEntityRenderDetails> ExecuteAsync(GetCustomEntityRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbQuery = _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .FilterByCustomEntityId(query.CustomEntityId)
                .FilterByWorkFlowStatusQuery(query.WorkFlowStatus, query.CustomEntityVersionId)
                .Include(e => e.CustomEntity)
                .Include(e => e.CustomEntity.Locale);

            var dbResult = await dbQuery.FirstOrDefaultAsync();
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(dbResult.CustomEntity.CustomEntityDefinitionCode);

            var entity = Mapper.Map<CustomEntityRenderDetails>(dbResult);
            entity.Model = _customEntityDataModelMapper.Map(dbResult.CustomEntity.CustomEntityDefinitionCode, dbResult.SerializedData);

            entity.Sections = await _dbContext
                .PageTemplateSections
                .AsNoTracking()
                .Where(s => s.PageTemplateId == query.PageTemplateId)
                .ProjectTo<CustomEntityPageSectionRenderDetails>()
                .ToListAsync();

            var dbModules = await _dbContext
                .CustomEntityVersionPageModules
                .AsNoTracking()
                .Where(m => m.CustomEntityVersionId == entity.CustomEntityVersionId)
                .ToListAsync();

            await _entityVersionPageModuleMapper.MapSections<CustomEntityVersionPageModuleRenderDetails>(dbModules, entity.Sections, query.WorkFlowStatus, executionContext);

            return entity;
        }

        #endregion
    }
}
