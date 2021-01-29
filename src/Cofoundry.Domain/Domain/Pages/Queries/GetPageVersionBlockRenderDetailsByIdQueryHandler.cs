using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns data for a specific block in a page version by it's id. Because
    /// the mapped display model may contain other versioned entities, you can 
    /// optionally pass down a PublishStatusQuery to use in the mapping process.
    /// </summary>
    public class GetPageVersionBlockRenderDetailsByIdQueryHandler
        : IQueryHandler<GetPageVersionBlockRenderDetailsByIdQuery, PageVersionBlockRenderDetails>
        , IPermissionRestrictedQueryHandler<GetPageVersionBlockRenderDetailsByIdQuery, PageVersionBlockRenderDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;

        public GetPageVersionBlockRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageVersionBlockModelMapper pageVersionBlockModelMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
        }

        #endregion

        #region execution

        public async Task<PageVersionBlockRenderDetails> ExecuteAsync(GetPageVersionBlockRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await QueryBlock(query.PageVersionBlockId)
                .Select(b => new
                {
                    PageBlock = b,
                    BlockTypeFileName = b.PageBlockType.FileName
                })
                .SingleOrDefaultAsync();

            if (dbResult == null) return null;

            var result = await MapAsync(
                dbResult.PageBlock, 
                dbResult.BlockTypeFileName, 
                query.PublishStatus,
                executionContext
                );

            // Add any list context information.
            var displayData = result.DisplayModel as IListablePageBlockTypeDisplayModel;

            if (displayData != null)
            {
                var blocks = await GetOrderedBlockIds(dbResult.PageBlock).ToListAsync();

                displayData.ListContext = new ListablePageBlockRenderContext()
                {
                    Index = blocks.IndexOf(result.PageVersionBlockId),
                    NumBlocks = blocks.Count
                };
            }

            return result;
        }

        private IQueryable<int> GetOrderedBlockIds(PageVersionBlock pageVersionBlock)
        {
            return _dbContext
                .PageVersionBlocks
                .AsNoTracking()
                .FilterActive()
                .Where(m => m.PageVersionId == pageVersionBlock.PageVersionId && m.PageTemplateRegionId == pageVersionBlock.PageTemplateRegionId)
                .OrderBy(m => m.Ordering)
                .Select(m => m.PageVersionBlockId);
        }

        private async Task<PageVersionBlockRenderDetails> MapAsync(
            PageVersionBlock pageVersionBlock, 
            string blockTypeFileName, 
            PublishStatusQuery publishStatus,
            IExecutionContext executionContext
            )
        {
            var blockTypeQuery = new GetPageBlockTypeSummaryByIdQuery(pageVersionBlock.PageBlockTypeId);
            var blockType = await _queryExecutor.ExecuteAsync(blockTypeQuery, executionContext);
            EntityNotFoundException.ThrowIfNull(blockType, pageVersionBlock.PageBlockTypeId);

            var result = new PageVersionBlockRenderDetails();
            result.PageVersionBlockId = pageVersionBlock.PageVersionBlockId;
            result.BlockType = blockType;
            result.DisplayModel = await _pageVersionBlockModelMapper.MapDisplayModelAsync(
                blockTypeFileName, 
                pageVersionBlock, 
                publishStatus,
                executionContext
                );
            
            return result;
        }

        private IQueryable<PageVersionBlock> QueryBlock(int pageVersionBlockId)
        {
            return _dbContext
                .PageVersionBlocks
                .AsNoTracking()
                .FilterActive()
                .Where(m => m.PageVersionBlockId == pageVersionBlockId);
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageVersionBlockRenderDetailsByIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
