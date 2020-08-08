using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    public class GetUpdateSeoSiteSettingsCommandQueryHandler 
        : IQueryHandler<GetUpdateCommandQuery<UpdateSeoSettingsCommand>, UpdateSeoSettingsCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetUpdateSeoSiteSettingsCommandQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<UpdateSeoSettingsCommand> ExecuteAsync(GetUpdateCommandQuery<UpdateSeoSettingsCommand> query, IExecutionContext executionContext)
        {
            var settings = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<SeoSettings>(), executionContext);

            return new UpdateSeoSettingsCommand()
            {
                HumansTxt = settings.HumansTxt,
                RobotsTxt = settings.RobotsTxt
            };
        }
    }
}
