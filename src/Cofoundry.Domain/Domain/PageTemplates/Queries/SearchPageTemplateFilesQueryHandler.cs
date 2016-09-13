using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class SearchPageTemplateFilesQueryHandler 
        : IAsyncQueryHandler<SearchPageTemplateFilesQuery, PagedQueryResult<PageTemplateFile>>
        , IPermissionRestrictedQueryHandler<SearchPageTemplateFilesQuery, PagedQueryResult<PageTemplateFile>>
    {
        #region constructor

        private readonly IPageTemplateViewFileLocator _viewLocator;
        private readonly IQueryExecutor _queryExecutor;

        public SearchPageTemplateFilesQueryHandler(
            IPageTemplateViewFileLocator viewLocator,
            IQueryExecutor queryExecutor
            )
        {
            _viewLocator = viewLocator;
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public async Task<PagedQueryResult<PageTemplateFile>> ExecuteAsync(SearchPageTemplateFilesQuery query, IExecutionContext executionContext)
        {
            var files = _viewLocator
                .GetPageTemplateFiles(query.Name);

            if (query.ExcludeRegistered)
            {
                var allLayouts = await _queryExecutor.GetAllAsync<PageTemplateMicroSummary>();
                files = files.Where(f => !allLayouts.Any(l => f.FullPath.Equals(l.FullPath, StringComparison.OrdinalIgnoreCase)));
            }

            var pagedResults = files
                .AsQueryable()
                .ToPagedResult(query);

            return pagedResults;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(SearchPageTemplateFilesQuery query)
        {
            yield return new CompositePermissionApplication(new PageTemplateCreatePermission(), new PageTemplateUpdatePermission());
        }

        #endregion
    }
}
