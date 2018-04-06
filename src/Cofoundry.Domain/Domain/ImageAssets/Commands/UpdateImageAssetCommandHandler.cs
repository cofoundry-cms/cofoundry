using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core.MessageAggregator;

namespace Cofoundry.Domain
{
    public class UpdateImageAssetCommandHandler 
        : IAsyncCommandHandler<UpdateImageAssetCommand>
        , IPermissionRestrictedCommandHandler<UpdateImageAssetCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly EntityTagHelper _entityTagHelper;
        private readonly IImageAssetFileService _imageAssetFileService;
        private readonly IImageAssetCache _imageAssetCache;
        private readonly IResizedImageAssetFileService _imageAssetFileCache;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IMessageAggregator _messageAggregator;

        public UpdateImageAssetCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityTagHelper entityTagHelper,
            IImageAssetFileService imageAssetFileService,
            IImageAssetCache imageAssetCache,
            IResizedImageAssetFileService imageAssetFileCache,
            ITransactionScopeFactory transactionScopeFactory,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityTagHelper = entityTagHelper;
            _imageAssetFileService = imageAssetFileService;
            _imageAssetCache = imageAssetCache;
            _imageAssetFileCache = imageAssetFileCache;
            _transactionScopeFactory = transactionScopeFactory;
            _messageAggregator = messageAggregator;
        }

        #endregion

        #region Execute

        public async Task ExecuteAsync(UpdateImageAssetCommand command, IExecutionContext executionContext)
        {
            bool hasNewFile = command.File != null;

            var imageAsset = await _dbContext
                .ImageAssets
                .Include(a => a.ImageAssetTags)
                .ThenInclude(a => a.Tag)
                .FilterById(command.ImageAssetId)
                .SingleOrDefaultAsync();

            imageAsset.FileDescription = command.Title;
            imageAsset.FileName = SlugFormatter.ToSlug(command.Title);
            imageAsset.DefaultAnchorLocation = command.DefaultAnchorLocation;
            _entityTagHelper.UpdateTags(imageAsset.ImageAssetTags, command.Tags, executionContext);
            _entityAuditHelper.SetUpdated(imageAsset, executionContext);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                if (hasNewFile)
                {
                    await _imageAssetFileService.SaveAsync(command.File, imageAsset, nameof(command.File));
                }

                await _dbContext.SaveChangesAsync();

                scope.Complete();
            }

            if (hasNewFile)
            {
                await _imageAssetFileCache.ClearAsync(imageAsset.ImageAssetId);
            }
            _imageAssetCache.Clear(imageAsset.ImageAssetId);

            await _messageAggregator.PublishAsync(new ImageAssetUpdatedMessage()
            {
                ImageAssetId = imageAsset.ImageAssetId,
                HasFileChanged = hasNewFile
            });
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateImageAssetCommand command)
        {
            yield return new ImageAssetUpdatePermission();
        }

        #endregion
    }
}
