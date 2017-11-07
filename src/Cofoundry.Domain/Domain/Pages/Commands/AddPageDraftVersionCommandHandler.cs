using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.EntityFramework;
using System.Data.SqlClient;
using Cofoundry.Core.MessageAggregator;

namespace Cofoundry.Domain
{
    public class AddPageDraftVersionCommandHandler 
        : IAsyncCommandHandler<AddPageDraftVersionCommand>
        , IPermissionRestrictedCommandHandler<AddPageDraftVersionCommand>
    {
        #region constructor

        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPageStoredProcedures _pageStoredProcedures;

        public AddPageDraftVersionCommandHandler(
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            IPageStoredProcedures pageStoredProcedures
            )
        {
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _pageStoredProcedures = pageStoredProcedures;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(AddPageDraftVersionCommand command, IExecutionContext executionContext)
        {
            var newVersionId = await _pageStoredProcedures.AddDraftAsync(
                command.PageId,
                command.CopyFromPageVersionId,
                executionContext.ExecutionDate,
                executionContext.UserContext.UserId.Value);

            _pageCache.Clear(command.PageId);

            // Set Ouput
            command.OutputPageVersionId = newVersionId;

            await _messageAggregator.PublishAsync(new PageDraftVersionAddedMessage()
            {
                PageId = command.PageId,
                PageVersionId = newVersionId
            });
        }

        #endregion

        #region Permissions

        public IEnumerable<IPermissionApplication> GetPermissions(AddPageDraftVersionCommand command)
        {
            yield return new PageUpdatePermission();
        }

        #endregion
    }
}
