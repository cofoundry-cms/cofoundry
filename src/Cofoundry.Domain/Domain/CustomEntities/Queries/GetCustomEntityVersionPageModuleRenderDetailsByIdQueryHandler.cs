using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetCustomEntityVersionPageModuleRenderDetailsByIdQueryHandler
        : IAsyncQueryHandler<GetCustomEntityVersionPageModuleRenderDetailsByIdQuery, CustomEntityVersionPageModuleRenderDetails>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageVersionModuleModelMapper _pageVersionModuleModelMapper;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityVersionPageModuleRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageVersionModuleModelMapper pageVersionModuleModelMapper,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageVersionModuleModelMapper = pageVersionModuleModelMapper;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<CustomEntityVersionPageModuleRenderDetails> ExecuteAsync(GetCustomEntityVersionPageModuleRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await QueryModule(query.CustomEntityVersionPageModuleId)
                .Select(m => new
                {
                    PageModule = m,
                    ModuleTypeFileName = m.PageModuleType.FileName,
                    CustomEntityDefinitionCode = m.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode
                })
                .SingleOrDefaultAsync();

            if (dbResult == null) return null;

            var result = await MapAsync(dbResult.PageModule, dbResult.ModuleTypeFileName, dbResult.CustomEntityDefinitionCode, query.WorkFlowStatus);

            // Add any list context information.
            var displayData = result.DisplayModel as IListablePageModuleDisplayModel;

            if (displayData != null)
            {
                var modules = await GetOrderedModuleIds(dbResult.PageModule).ToListAsync();

                displayData.ListContext = new ListablePageModuleRenderContext()
                {
                    Index = modules.IndexOf(result.CustomEntityVersionPageModuleId),
                    NumModules = modules.Count
                };
            }

            return result;
        }

        #endregion

        #region private helpers

        private IQueryable<int> GetOrderedModuleIds(CustomEntityVersionPageModule versionModule)
        {
            return _dbContext
                .CustomEntityVersionPageModules
                .AsNoTracking()
                .Where(m => m.CustomEntityVersionId == versionModule.CustomEntityVersionId && m.PageTemplateSectionId == versionModule.PageTemplateSectionId)
                .OrderBy(m => m.Ordering)
                .Select(m => m.CustomEntityVersionPageModuleId);
        }

        private async Task<CustomEntityVersionPageModuleRenderDetails> MapAsync(CustomEntityVersionPageModule versionModule, string moduleTypeFileName, string customEntityDefinitionCode, WorkFlowStatusQuery workflowStatus)
        {
            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityReadPermission>(customEntityDefinitionCode);

            var moduleType = await _queryExecutor.GetByIdAsync<PageModuleTypeSummary>(versionModule.PageModuleTypeId);
            EntityNotFoundException.ThrowIfNull(moduleType, versionModule.PageModuleTypeId);

            var result = new CustomEntityVersionPageModuleRenderDetails();
            result.CustomEntityVersionPageModuleId = versionModule.CustomEntityVersionPageModuleId;
            result.ModuleType = moduleType;
            result.DisplayModel = await _pageVersionModuleModelMapper.MapDisplayModelAsync(moduleTypeFileName, versionModule, workflowStatus);
            
            return result;
        }

        private IQueryable<CustomEntityVersionPageModule> QueryModule(int customEntityVersionPageModuleId)
        {
            return _dbContext
                .CustomEntityVersionPageModules
                .AsNoTracking()
                .Where(m => m.CustomEntityVersionPageModuleId == customEntityVersionPageModuleId);
        }

        /// <summary>
        /// Gets the custom view template to render the module as if one
        /// is assigned, otherwise null.
        /// </summary>
        public PageModuleTypeTemplateSummary GetCustomTemplate(CustomEntityVersionPageModule pageModule, CustomEntityVersionPageModuleRenderDetails moduleRenderDetails)
        {
            if (!pageModule.PageModuleTypeTemplateId.HasValue) return null;

            var template = moduleRenderDetails
                .ModuleType
                .Templates
                .FirstOrDefault(t => t.PageModuleTypeTemplateId == pageModule.PageModuleTypeTemplateId);

            Debug.Assert(template != null, string.Format("The module template with id {0} could not be found for custom entity module {1}", pageModule.PageModuleTypeTemplateId, pageModule.CustomEntityVersionPageModuleId));

            return template;
        }

        #endregion
    }
}
