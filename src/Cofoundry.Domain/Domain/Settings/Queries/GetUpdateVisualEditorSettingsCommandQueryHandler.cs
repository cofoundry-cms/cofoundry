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
    public class GetUpdateVisualEditorSettingsCommandQueryHandler
        : IAsyncQueryHandler<GetQuery<UpdateVisualEditorSettingsCommand>, UpdateVisualEditorSettingsCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetUpdateVisualEditorSettingsCommandQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<UpdateVisualEditorSettingsCommand> ExecuteAsync(GetQuery<UpdateVisualEditorSettingsCommand> query, IExecutionContext executionContext)
        {
            var settings = await _queryExecutor.GetAsync<VisualEditorSettings>();

            return Mapper.Map<UpdateVisualEditorSettingsCommand>(settings);
        }
    }
}
