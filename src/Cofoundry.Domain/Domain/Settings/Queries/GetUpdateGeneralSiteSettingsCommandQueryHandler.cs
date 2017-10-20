using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetUpdateGeneralSiteSettingsCommandQueryHandler 
        : IAsyncQueryHandler<GetQuery<UpdateGeneralSiteSettingsCommand>, UpdateGeneralSiteSettingsCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetUpdateGeneralSiteSettingsCommandQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<UpdateGeneralSiteSettingsCommand> ExecuteAsync(GetQuery<UpdateGeneralSiteSettingsCommand> query, IExecutionContext executionContext)
        {
            var settings = await _queryExecutor.GetAsync<GeneralSiteSettings>();

            return new UpdateGeneralSiteSettingsCommand()
            {
                AllowAutomaticUpdates = settings.AllowAutomaticUpdates,
                ApplicationName = settings.ApplicationName
            };
        }
    }
}
