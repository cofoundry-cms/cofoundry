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
    public class GetUpdateSeoSiteSettingsCommandQueryHandler 
        : IAsyncQueryHandler<GetQuery<UpdateSeoSettingsCommand>, UpdateSeoSettingsCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetUpdateSeoSiteSettingsCommandQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<UpdateSeoSettingsCommand> ExecuteAsync(GetQuery<UpdateSeoSettingsCommand> query, IExecutionContext executionContext)
        {
            var settings = await _queryExecutor.GetAsync<SeoSettings>();

            return Mapper.Map<UpdateSeoSettingsCommand>(settings);
        }
    }
}
