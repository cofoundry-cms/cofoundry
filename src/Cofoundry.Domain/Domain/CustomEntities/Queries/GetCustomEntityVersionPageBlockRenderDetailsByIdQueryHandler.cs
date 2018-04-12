using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetCustomEntityVersionPageBlockRenderDetailsByIdQueryHandler
        : IAsyncQueryHandler<GetCustomEntityVersionPageBlockRenderDetailsByIdQuery, CustomEntityVersionPageBlockRenderDetails>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityVersionPageBlockRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageVersionBlockModelMapper pageVersionBlockModelMapper,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<CustomEntityVersionPageBlockRenderDetails> ExecuteAsync(GetCustomEntityVersionPageBlockRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await QueryBlock(query.CustomEntityVersionPageBlockId)
                .Select(b => new
                {
                    PageBlock = b,
                    BlockTypeFileName = b.PageBlockType.FileName,
                    CustomEntityDefinitionCode = b.CustomEntityVersion.CustomEntity.CustomEntityDefinitionCode
                })
                .SingleOrDefaultAsync();

            if (dbResult == null) return null;

            var result = await MapAsync(
                dbResult.PageBlock, 
                dbResult.BlockTypeFileName, 
                dbResult.CustomEntityDefinitionCode, 
                query.PublishStatus,
                executionContext
                );

            // Add any list context information.

            if (result.DisplayModel is IListablePageBlockTypeDisplayModel displayData)
            {
                var blocks = await GetOrderedBlockIds(dbResult.PageBlock).ToListAsync();

                displayData.ListContext = new ListablePageBlockRenderContext()
                {
                    Index = blocks.IndexOf(result.CustomEntityVersionPageBlockId),
                    NumBlocks = blocks.Count
                };
            }

            return result;
        }

        private IQueryable<int> GetOrderedBlockIds(CustomEntityVersionPageBlock versionBlock)
        {
            return _dbContext
                .CustomEntityVersionPageBlocks
                .AsNoTracking()
                .FilterActive()
                .Where(m => m.CustomEntityVersionId == versionBlock.CustomEntityVersionId 
                    && m.PageTemplateRegionId == versionBlock.PageTemplateRegionId)
                .OrderBy(m => m.Ordering)
                .Select(m => m.CustomEntityVersionPageBlockId);
        }

        private async Task<CustomEntityVersionPageBlockRenderDetails> MapAsync(
            CustomEntityVersionPageBlock versionBlock, 
            string blockTypeFileName, 
            string customEntityDefinitionCode, 
            PublishStatusQuery publishStatus,
            IExecutionContext executionContext
            )
        {
            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityReadPermission>(customEntityDefinitionCode);

            var blockTypeQuery = new GetPageBlockTypeSummaryByIdQuery(versionBlock.PageBlockTypeId);
            var blockType = await _queryExecutor.ExecuteAsync(blockTypeQuery, executionContext);
            EntityNotFoundException.ThrowIfNull(blockType, versionBlock.PageBlockTypeId);

            var result = new CustomEntityVersionPageBlockRenderDetails();
            result.CustomEntityVersionPageBlockId = versionBlock.CustomEntityVersionPageBlockId;
            result.BlockType = blockType;
            result.DisplayModel = await _pageVersionBlockModelMapper.MapDisplayModelAsync(blockTypeFileName, versionBlock, publishStatus);
            
            return result;
        }

        private IQueryable<CustomEntityVersionPageBlock> QueryBlock(int customEntityVersionPageBlockId)
        {
            return _dbContext
                .CustomEntityVersionPageBlocks
                .AsNoTracking()
                .FilterActive()
                .Where(m => m.CustomEntityVersionPageBlockId == customEntityVersionPageBlockId);
        }

        /// <summary>
        /// Gets the custom view template to render the block as if one
        /// is assigned, otherwise null.
        /// </summary>
        public PageBlockTypeTemplateSummary GetCustomTemplate(CustomEntityVersionPageBlock pageBlock, CustomEntityVersionPageBlockRenderDetails blockRenderDetails)
        {
            if (!pageBlock.PageBlockTypeTemplateId.HasValue) return null;

            var template = blockRenderDetails
                .BlockType
                .Templates
                .FirstOrDefault(t => t.PageBlockTypeTemplateId == pageBlock.PageBlockTypeTemplateId);

            Debug.Assert(template != null, string.Format("The page block type template with id {0} could not be found for custom entity block {1}", pageBlock.PageBlockTypeTemplateId, pageBlock.CustomEntityVersionPageBlockId));

            return template;
        }

        #endregion
    }
}
