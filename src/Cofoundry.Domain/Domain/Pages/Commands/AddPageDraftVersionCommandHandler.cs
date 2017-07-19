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
        : ICommandHandler<AddPageDraftVersionCommand>
        , IAsyncCommandHandler<AddPageDraftVersionCommand>
        , IPermissionRestrictedCommandHandler<AddPageDraftVersionCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IEntityFrameworkSqlExecutor _entityFrameworkSqlExecutor;

        public AddPageDraftVersionCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            IEntityFrameworkSqlExecutor entityFrameworkSqlExecutor
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _entityFrameworkSqlExecutor = entityFrameworkSqlExecutor;
        }

        #endregion

        #region execution

        public void Execute(AddPageDraftVersionCommand command, IExecutionContext executionContext)
        {
            var newVersionId = _entityFrameworkSqlExecutor
                .ExecuteCommandWithOutput<int?>(_dbContext,
                "Cofoundry.Page_AddDraft",
                "PageVersionId",
                 new SqlParameter("PageId", command.PageId),
                 new SqlParameter("CopyFromPageVersionId", command.CopyFromPageVersionId),
                 new SqlParameter("CreateDate", executionContext.ExecutionDate),
                 new SqlParameter("CreatorId", executionContext.UserContext.UserId)
                 );

            if (!newVersionId.HasValue)
            {
                throw new UnexpectedSqlStoredProcedureResultException("Cofoundry.Page_AddDraft", "No PageId was returned.");
            }

            _pageCache.Clear(newVersionId.Value);

            // Set Ouput
            command.OutputPageVersionId = newVersionId.Value;
        }

        public async Task ExecuteAsync(AddPageDraftVersionCommand command, IExecutionContext executionContext)
        {
            var newVersionId = await _entityFrameworkSqlExecutor
                .ExecuteCommandWithOutputAsync<int?>(_dbContext,
                "Cofoundry.Page_AddDraft",
                "PageVersionId",
                 new SqlParameter("PageId", command.PageId),
                 new SqlParameter("CopyFromPageVersionId", command.CopyFromPageVersionId),
                 new SqlParameter("CreateDate", executionContext.ExecutionDate),
                 new SqlParameter("CreatorId", executionContext.UserContext.UserId)
                 );

            if (!newVersionId.HasValue)
            {
                throw new UnexpectedSqlStoredProcedureResultException("Cofoundry.Page_AddDraft", "No PageId was returned.");
            }

            _pageCache.Clear(command.PageId);

            // Set Ouput
            command.OutputPageVersionId = newVersionId.Value;

            await _messageAggregator.PublishAsync(new PageDraftVersionAddedMessage()
            {
                PageId = command.PageId,
                PageVersionId = newVersionId.Value
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
