using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core;
using Cofoundry.Core.MessageAggregator;

namespace Cofoundry.Domain
{
    public class AddDocumentAssetCommandHandler 
        : IAsyncCommandHandler<AddDocumentAssetCommand>
        , IPermissionRestrictedCommandHandler<AddDocumentAssetCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityTagHelper _entityTagHelper;
        private readonly DocumentAssetCommandHelper _documentAssetCommandHelper;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IMessageAggregator _messageAggregator;

        public AddDocumentAssetCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityTagHelper entityTagHelper,
            DocumentAssetCommandHelper documentAssetCommandHelper,
            ITransactionScopeFactory transactionScopeFactory,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityTagHelper = entityTagHelper;
            _documentAssetCommandHelper = documentAssetCommandHelper;
            _transactionScopeFactory = transactionScopeFactory;
            _messageAggregator = messageAggregator;
        }

        #endregion

        #region Execute

        public async Task ExecuteAsync(AddDocumentAssetCommand command, IExecutionContext executionContext)
        {
            var documentAsset = new DocumentAsset();

            documentAsset.Title = command.Title;
            documentAsset.Description = command.Description ?? string.Empty;
            documentAsset.FileName = SlugFormatter.ToSlug(command.Title);
            _entityTagHelper.UpdateTags(documentAsset.DocumentAssetTags, command.Tags, executionContext);
            _entityAuditHelper.SetCreated(documentAsset, executionContext);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                _dbContext.DocumentAssets.Add(documentAsset);

                await _documentAssetCommandHelper.SaveFile(command.File, documentAsset);

                command.OutputDocumentAssetId = documentAsset.DocumentAssetId;
                scope.Complete();
            }

            await _messageAggregator.PublishAsync(new DocumentAssetAddedMessage()
            {
                DocumentAssetId = documentAsset.DocumentAssetId
            });
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddDocumentAssetCommand command)
        {
            yield return new DocumentAssetCreatePermission();
        }

        #endregion
    }
}
