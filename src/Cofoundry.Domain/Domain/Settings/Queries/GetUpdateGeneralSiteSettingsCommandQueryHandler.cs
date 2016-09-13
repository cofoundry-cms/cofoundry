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

            return Mapper.Map<UpdateGeneralSiteSettingsCommand>(settings);
        }
    }
}
