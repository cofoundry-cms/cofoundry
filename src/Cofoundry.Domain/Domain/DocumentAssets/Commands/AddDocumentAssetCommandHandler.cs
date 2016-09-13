using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core;

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

        public AddDocumentAssetCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityTagHelper entityTagHelper,
            DocumentAssetCommandHelper documentAssetCommandHelper,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityTagHelper = entityTagHelper;
            _documentAssetCommandHelper = documentAssetCommandHelper;
            _transactionScopeFactory = transactionScopeFactory;
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

            using (var scope = _transactionScopeFactory.Create())
            {
                _dbContext.DocumentAssets.Add(documentAsset);

                await _documentAssetCommandHelper.SaveFile(command.File, documentAsset);

                command.OutputDocumentAssetId = documentAsset.DocumentAssetId;
                scope.Complete();
            }
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
