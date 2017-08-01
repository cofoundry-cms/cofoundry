using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<CustomEntityDetails>, CustomEntityDetails>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
        private readonly IPageVersionModuleModelMapper _pageVersionModuleModelMapper;
        private readonly IEntityVersionPageModuleMapper _entityVersionPageModuleMapper;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            IPageVersionModuleModelMapper pageVersionModuleModelMapper,
            IEntityVersionPageModuleMapper entityVersionPageModuleMapper,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
            _pageVersionModuleModelMapper = pageVersionModuleModelMapper;
            _entityVersionPageModuleMapper = entityVersionPageModuleMapper;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<CustomEntityDetails> ExecuteAsync(GetByIdQuery<CustomEntityDetails> query, IExecutionContext executionContext)
        {
            var customEntityVersion = await Query(query.Id).FirstOrDefaultAsync();
            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityReadPermission>(customEntityVersion.CustomEntity.CustomEntityDefinitionCode);
            
            return await Map(query, customEntityVersion, executionContext);
        }

        #endregion

        #region helpers

        private async Task<CustomEntityDetails> Map(
            GetByIdQuery<CustomEntityDetails> query, 
            CustomEntityVersion dbVersion,
            IExecutionContext executionContext)
        {
            if (dbVersion == null) return null;

            var entity = Mapper.Map<CustomEntityDetails>(dbVersion.CustomEntity);
            entity.LatestVersion = Mapper.Map<CustomEntityVersionDetails>(dbVersion);
            entity.HasDraft = entity.LatestVersion.WorkFlowStatus == WorkFlowStatus.Draft;
            entity.IsPublished = entity.LatestVersion.WorkFlowStatus == WorkFlowStatus.Published;

            if (!entity.IsPublished)
            {
                entity.IsPublished = await _dbContext
                    .CustomEntityVersions
                    .AnyAsync(v => v.CustomEntityId == query.Id && v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
            }

            // Custom Mapping
            await MapDataModelAsync(query, dbVersion, entity.LatestVersion);
            
            await MapPages(dbVersion, entity, executionContext);

            return entity;
        }

        private async Task MapPages(CustomEntityVersion dbVersion, CustomEntityDetails entity, IExecutionContext executionContext)
        {
            var pages = new List<CustomEntityPage>();
            entity.LatestVersion.Pages = pages;

            var routingsQuery = new GetPageRoutingInfoByCustomEntityIdQuery(dbVersion.CustomEntityId);
            var routings = (await _queryExecutor.ExecuteAsync(routingsQuery, executionContext))
                .Where(r => r.CustomEntityRouteRule != null);

            if (!routings.Any()) return;
            
            // Map templates

            var pageTemplateIds = routings
                .Select(r => new
                {
                    PageId = r.PageRoute.PageId,
                    VersionRoute = r.PageRoute.Versions.GetVersionRouting(WorkFlowStatusQuery.Latest)
                })
                .Where(r => r.VersionRoute != null && r.VersionRoute.HasCustomEntityModuleSections)
                .ToDictionary(k => k.PageId, v => v.VersionRoute.PageTemplateId);

            var allTemplateIds = pageTemplateIds
                .Select(r => r.Value)
                .ToArray();

            var allTemplateSections = await _dbContext
                .PageTemplateSections
                .AsNoTracking()
                .Where(s => allTemplateIds.Contains(s.PageTemplateId) && s.IsCustomEntitySection)
                .ToListAsync();

            var allModuleTypes = await _queryExecutor.GetAllAsync<PageModuleTypeSummary>(executionContext);

            foreach (var routing in routings)
            {
                var page = new CustomEntityPage();
                pages.Add(page);
                page.FullPath = routing.CustomEntityRouteRule.MakeUrl(routing.PageRoute, routing.CustomEntityRoute);
                page.PageRoute = routing.PageRoute;

                // Map Sections

                var templateId = pageTemplateIds.GetOrDefault(routing.PageRoute.PageId);
                page.Sections = allTemplateSections
                    .Where(s => s.PageTemplateId == templateId)
                    .OrderBy(s => s.UpdateDate)
                    .Select(s => new CustomEntityPageSectionDetails()
                    {
                        Name = s.Name,
                        PageTemplateSectionId = s.PageTemplateSectionId
                    })
                    .ToList();

                // Map Modules

                foreach (var section in page.Sections)
                {
                    section.Modules = dbVersion
                        .CustomEntityVersionPageModules
                        .Where(m => m.PageTemplateSectionId == section.PageTemplateSectionId)
                        .OrderBy(m => m.Ordering)
                        .Select(m => MapModule(m, allModuleTypes))
                        .ToArray();
                }
            }

            // Map default full path

            entity.FullPath = pages
                .OrderByDescending(p => p.PageRoute.Locale == null)
                .Select(p => p.FullPath)
                .First();
        }

        private CustomEntityVersionPageModuleDetails MapModule(CustomEntityVersionPageModule dbModule, IEnumerable<PageModuleTypeSummary> allModuleTypes)
        {
            var moduleType = allModuleTypes.SingleOrDefault(t => t.PageModuleTypeId == dbModule.PageModuleTypeId);

            var module = new CustomEntityVersionPageModuleDetails();
            module.ModuleType = moduleType;
            module.DataModel = _pageVersionModuleModelMapper.MapDataModel(moduleType.FileName, dbModule);
            module.CustomEntityVersionPageModuleId = dbModule.CustomEntityVersionPageModuleId;
            module.Template = _entityVersionPageModuleMapper.GetCustomTemplate(dbModule, moduleType);

            return module;
        }

        private IQueryable<CustomEntityVersion> Query(int id)
        {
            return _dbContext
                .CustomEntityVersions
                .Include(v => v.CustomEntityVersionPageModules)
                .Include(v => v.CustomEntity)
                .ThenInclude(e => e.Creator)
                .Include(v => v.CustomEntity)
                .ThenInclude(e => e.Locale)
                .Include(v => v.Creator)
                .AsNoTracking()
                .Where(v => v.CustomEntityId == id && (v.CustomEntity.LocaleId == null || v.CustomEntity.Locale.IsActive))
                .OrderByDescending(g => g.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(g => g.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                .ThenByDescending(g => g.CreateDate);
        }

        private async Task MapDataModelAsync(GetByIdQuery<CustomEntityDetails> query, CustomEntityVersion dbVersion, CustomEntityVersionDetails version)
        {
            var definition = await _queryExecutor.GetByIdAsync<CustomEntityDefinitionSummary>(dbVersion.CustomEntity.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, dbVersion.CustomEntity.CustomEntityDefinitionCode);

            version.Model = (ICustomEntityDataModel)_dbUnstructuredDataSerializer.Deserialize(dbVersion.SerializedData, definition.DataModelType);
        }

        #endregion
    }
}
