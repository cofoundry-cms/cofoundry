using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;

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
        private readonly ImageAssetCommandHelper _imageAssetCommandHelper;
        private readonly IImageAssetCache _imageAssetCache;
        private readonly IResizedImageAssetFileService _imageAssetFileCache;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public UpdateImageAssetCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            EntityTagHelper entityTagHelper,
            ImageAssetCommandHelper imageAssetCommandHelper,
            IImageAssetCache imageAssetCache,
            IResizedImageAssetFileService imageAssetFileCache,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _entityTagHelper = entityTagHelper;
            _imageAssetCommandHelper = imageAssetCommandHelper;
            _imageAssetCache = imageAssetCache;
            _imageAssetFileCache = imageAssetFileCache;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region Execute
        
        public async Task ExecuteAsync(UpdateImageAssetCommand command, IExecutionContext executionContext)
        {
            bool hasNewFile = command.File != null;

            var imageAsset = await _dbContext
                .ImageAssets
                .Include(a => a.ImageAssetTags)
                .Include(a => a.ImageAssetTags.Select(pt => pt.Tag))
                .FilterById(command.ImageAssetId)
                .SingleOrDefaultAsync();

            imageAsset.FileDescription = command.Title;
            imageAsset.FileName = SlugFormatter.ToSlug(command.Title);
            imageAsset.DefaultAnchorLocation = command.DefaultAnchorLocation;
            _entityTagHelper.UpdateTags(imageAsset.ImageAssetTags, command.Tags, executionContext);
            _entityAuditHelper.SetUpdated(imageAsset, executionContext);

            using (var scope = _transactionScopeFactory.Create())
            {
                if (hasNewFile)
                {
                    await _imageAssetCommandHelper.SaveFile(command.File, imageAsset);
                }

                await _dbContext.SaveChangesAsync();

                scope.Complete();
            }

            if (hasNewFile)
            {
                _imageAssetFileCache.Clear(imageAsset.ImageAssetId);
            }
            _imageAssetCache.Clear(imageAsset.ImageAssetId);
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
