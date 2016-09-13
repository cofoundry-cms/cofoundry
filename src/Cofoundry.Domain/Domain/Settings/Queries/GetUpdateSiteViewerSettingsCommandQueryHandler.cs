using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class GetUpdateSiteViewerSettingsCommandQueryHandler 
        : IAsyncQueryHandler<GetQuery<UpdateSiteViewerSettingsCommand>, UpdateSiteViewerSettingsCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetUpdateSiteViewerSettingsCommandQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<UpdateSiteViewerSettingsCommand> ExecuteAsync(GetQuery<UpdateSiteViewerSettingsCommand> query, IExecutionContext executionContext)
        {
            var settings = await _queryExecutor.GetAsync<SiteViewerSettings>();

            return Mapper.Map<UpdateSiteViewerSettingsCommand>(settings);
        }
    }
}
