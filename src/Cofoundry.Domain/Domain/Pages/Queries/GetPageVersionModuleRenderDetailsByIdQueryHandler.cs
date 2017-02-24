using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Diagnostics;
using System.Data.Entity;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetPageVersionModuleRenderDetailsByIdQueryHandler
        : IQueryHandler<GetPageVersionModuleRenderDetailsByIdQuery, PageVersionModuleRenderDetails>
        , IAsyncQueryHandler<GetPageVersionModuleRenderDetailsByIdQuery, PageVersionModuleRenderDetails>
        , IPermissionRestrictedQueryHandler<GetPageVersionModuleRenderDetailsByIdQuery, PageVersionModuleRenderDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageVersionModuleModelMapper _pageVersionModuleModelMapper;

        public GetPageVersionModuleRenderDetailsByIdQueryHandler(
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

        #region execution

        public PageVersionModuleRenderDetails Execute(GetPageVersionModuleRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = QueryModule(query.PageVersionModuleId)
                .Select(m => new {
                    PageModule = m,
                    ModuleTypeFileName = m.PageModuleType.FileName
                })
                .SingleOrDefault();

            if (dbResult == null) return null; 
            
            var result = Map(dbResult.PageModule, dbResult.ModuleTypeFileName, query.WorkFlowStatus);
                        
            // Add any list context information.
            var displayData = result.DisplayModel as IListablePageModuleDisplayModel;

            if (displayData != null)
            {
                var modules = GetOrderedModuleIds(dbResult.PageModule).ToList();

                displayData.ListContext = new ListablePageModuleRenderContext()
                {
                    Index = modules.IndexOf(result.PageVersionModuleId),
                    NumModules = modules.Count
                };
            }

            return result;
        }

        public async Task<PageVersionModuleRenderDetails> ExecuteAsync(GetPageVersionModuleRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await QueryModule(query.PageVersionModuleId)
                .Select(m => new
                {
                    PageModule = m,
                    ModuleTypeFileName = m.PageModuleType.FileName
                })
                .SingleOrDefaultAsync();

            if (dbResult == null) return null;

            var result = Map(dbResult.PageModule, dbResult.ModuleTypeFileName, query.WorkFlowStatus);

            // Add any list context information.
            var displayData = result.DisplayModel as IListablePageModuleDisplayModel;

            if (displayData != null)
            {
                var modules = await GetOrderedModuleIds(dbResult.PageModule).ToListAsync();

                displayData.ListContext = new ListablePageModuleRenderContext()
                {
                    Index = modules.IndexOf(result.PageVersionModuleId),
                    NumModules = modules.Count
                };
            }

            return result;
        }

        #endregion

        #region private helpers

        private IQueryable<int> GetOrderedModuleIds(PageVersionModule pageVersionModule)
        {
            return _dbContext
                .PageVersionModules
                .AsNoTracking()
                .Where(m => m.PageVersionId == pageVersionModule.PageVersionId && m.PageTemplateSectionId == pageVersionModule.PageTemplateSectionId)
                .OrderBy(m => m.Ordering)
                .Select(m => m.PageVersionModuleId);
        }

        private PageVersionModuleRenderDetails Map(PageVersionModule pageVersionModule, string moduleTypeFileName, WorkFlowStatusQuery workFlowStatus)
        {
            var moduleType = _queryExecutor.GetById<PageModuleTypeSummary>(pageVersionModule.PageModuleTypeId);
            EntityNotFoundException.ThrowIfNull(moduleType, pageVersionModule.PageModuleTypeId);

            var result = new PageVersionModuleRenderDetails();
            result.PageVersionModuleId = pageVersionModule.PageVersionModuleId;
            result.ModuleType = moduleType;
            result.DisplayModel = _pageVersionModuleModelMapper.MapDisplayModel(moduleTypeFileName, pageVersionModule, workFlowStatus);
            
            return result;
        }

        private IQueryable<PageVersionModule> QueryModule(int pageVersionModuleId)
        {
            return _dbContext
                .PageVersionModules
                .AsNoTracking()
                .Where(m => m.PageVersionModuleId == pageVersionModuleId);
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageVersionModuleRenderDetailsByIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
