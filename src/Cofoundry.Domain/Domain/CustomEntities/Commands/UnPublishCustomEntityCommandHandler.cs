using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class UnPublishCustomEntityCommandHandler 
        : IAsyncCommandHandler<UnPublishCustomEntityCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly IPermissionValidationService _permissionValidationService;

        public UnPublishCustomEntityCommandHandler(
            CofoundryDbContext dbContext,
            ICustomEntityCache customEntityCache,
            IMessageAggregator messageAggregator,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _customEntityCache = customEntityCache;
            _messageAggregator = messageAggregator;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UnPublishCustomEntityCommand command, IExecutionContext executionContext)
        {
            var versions = await _dbContext
                .CustomEntityVersions
                .Include(v => v.CustomEntity)
                .Where(p => p.CustomEntityId == command.CustomEntityId
                    && (p.WorkFlowStatusId == (int)WorkFlowStatus.Published || p.WorkFlowStatusId == (int)WorkFlowStatus.Draft))
                .ToListAsync();

            var publishedVersion = versions.SingleOrDefault(p => p.WorkFlowStatusId == (int)WorkFlowStatus.Published);
            EntityNotFoundException.ThrowIfNull(publishedVersion, command.CustomEntityId);
            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityPublishPermission>(publishedVersion.CustomEntity.CustomEntityDefinitionCode);

            if (versions.Any(p => p.WorkFlowStatusId == (int)WorkFlowStatus.Draft))
            {
                // If there's already a draft, change to approved.
                publishedVersion.WorkFlowStatusId = (int)WorkFlowStatus.Approved;
            }
            else
            {
                // Else set it to draft
                publishedVersion.WorkFlowStatusId = (int)WorkFlowStatus.Draft;
            }

            await _dbContext.SaveChangesAsync();
            _customEntityCache.Clear(publishedVersion.CustomEntity.CustomEntityDefinitionCode, command.CustomEntityId);

            await _messageAggregator.PublishAsync(new CustomEntityUnPublishedMessage()
            {
                CustomEntityId = command.CustomEntityId,
                CustomEntityDefinitionCode = publishedVersion.CustomEntity.CustomEntityDefinitionCode
            });
        }

        #endregion
    }
}
