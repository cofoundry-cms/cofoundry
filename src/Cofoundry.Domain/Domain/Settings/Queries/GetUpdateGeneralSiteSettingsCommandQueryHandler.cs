using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    public class GetUpdateGeneralSiteSettingsCommandQueryHandler 
        : IQueryHandler<GetUpdateCommandQuery<UpdateGeneralSiteSettingsCommand>, UpdateGeneralSiteSettingsCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetUpdateGeneralSiteSettingsCommandQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<UpdateGeneralSiteSettingsCommand> ExecuteAsync(GetUpdateCommandQuery<UpdateGeneralSiteSettingsCommand> query, IExecutionContext executionContext)
        {
            var settings = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<GeneralSiteSettings>(), executionContext);

            return new UpdateGeneralSiteSettingsCommand()
            {
                AllowAutomaticUpdates = settings.AllowAutomaticUpdates,
                ApplicationName = settings.ApplicationName
            };
        }
    }
}
