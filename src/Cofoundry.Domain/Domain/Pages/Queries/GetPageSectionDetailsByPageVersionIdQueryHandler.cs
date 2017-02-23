using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Diagnostics;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a collection of the content managed sections and
    /// modules for a specific version of a page. These are the 
    /// modules that get rendered in the page template linked
    /// to the page version.
    /// </summary>
    public class GetPageSectionDetailsByPageVersionIdQueryHandler 
        : IQueryHandler<GetPageSectionDetailsByPageVersionIdQuery, IEnumerable<PageSectionDetails>>
        , IAsyncQueryHandler<GetPageSectionDetailsByPageVersionIdQuery, IEnumerable<PageSectionDetails>>
        , IPermissionRestrictedQueryHandler<GetPageSectionDetailsByPageVersionIdQuery, IEnumerable<PageSectionDetails>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageVersionModuleModelMapper _pageVersionModuleModelMapper;

        public GetPageSectionDetailsByPageVersionIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageVersionModuleModelMapper pageVersionModuleModelMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageVersionModuleModelMapper = pageVersionModuleModelMapper;
        }

        #endregion

        public IEnumerable<PageSectionDetails> Execute(GetPageSectionDetailsByPageVersionIdQuery query, IExecutionContext executionContext)
        {
            var sections = GetSections(query).ToList();
            var dbModules = QueryModules(query).ToList();
            var allModuleTypes = _queryExecutor.GetAll<PageModuleTypeSummary>(executionContext);

            MapSections(sections, dbModules, allModuleTypes);

            return sections;
        }

        public async Task<IEnumerable<PageSectionDetails>> ExecuteAsync(GetPageSectionDetailsByPageVersionIdQuery query, IExecutionContext executionContext)
        {
            var sections = await GetSections(query).ToListAsync();
            var dbModules = await QueryModules(query).ToListAsync();
            var allModuleTypes = await _queryExecutor.GetAllAsync<PageModuleTypeSummary>(executionContext);

            MapSections(sections, dbModules, allModuleTypes);

            return sections;
        }

        #region private helpers

        private void MapSections(List<PageSectionDetails> sections, List<PageVersionModule> dbModules, IEnumerable<PageModuleTypeSummary> allModuleTypes)
        {
            foreach (var section in sections)
            {
                var sectionMappedModules = dbModules
                    .Where(m => m.PageTemplateSectionId == section.PageTemplateSectionId)
                    .OrderBy(m => m.Ordering)
                    .Select(m => MapModule(m, allModuleTypes));

                section.Modules = sectionMappedModules.ToArray();
            }
        }

        private IQueryable<PageVersionModule> QueryModules(GetPageSectionDetailsByPageVersionIdQuery query)
        {
            return _dbContext
                .PageVersionModules
                .AsNoTracking()
                .Where(m => m.PageVersionId == query.PageVersionId);
        }

        private PageVersionModuleDetails MapModule(PageVersionModule dbModule, IEnumerable<PageModuleTypeSummary> allModuleTypes)
        {
            var moduleType = allModuleTypes.SingleOrDefault(t => t.PageModuleTypeId == dbModule.PageModuleTypeId);

            var module = new PageVersionModuleDetails();
            module.ModuleType = moduleType;
            module.DataModel = _pageVersionModuleModelMapper.MapDataModel(moduleType.FileName, dbModule);
            module.PageVersionModuleId = dbModule.PageVersionModuleId;
            module.Template = GetCustomTemplate(dbModule, moduleType);

            return module;
        }

        public PageModuleTypeTemplateSummary GetCustomTemplate(IEntityVersionPageModule pageModule, PageModuleTypeSummary moduleType)
        {
            if (!pageModule.PageModuleTypeTemplateId.HasValue) return null;

            var template = moduleType
                .Templates
                .FirstOrDefault(t => t.PageModuleTypeTemplateId == pageModule.PageModuleTypeTemplateId);

            Debug.Assert(template != null, string.Format("The module template with id {0} could not be found for {1} {2}", pageModule.PageModuleTypeTemplateId, pageModule.GetType().Name, pageModule.GetVersionModuleId()));

            return template;
        }

        private IQueryable<PageSectionDetails> GetSections(GetPageSectionDetailsByPageVersionIdQuery query)
        {
            var dbQuery = _dbContext
                .PageVersions
                .AsNoTracking()
                .Where(v => v.PageVersionId == query.PageVersionId && !v.IsDeleted)
                .SelectMany(v => v.PageTemplate.PageTemplateSections)
                .Where(s => !s.IsCustomEntitySection)
                .OrderBy(s => s.UpdateDate)
                .Select(s => new PageSectionDetails()
                {
                    PageTemplateSectionId = s.PageTemplateSectionId,
                    Name = s.Name
                });

            return dbQuery;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageSectionDetailsByPageVersionIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
