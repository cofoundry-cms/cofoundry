using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// A mapping helper containing a couple of mapping methods used in multiple queires
    /// to map page blocks in regular pages as well as custom entity details pages.
    /// </summary>
    public class EntityVersionPageBlockMapper : IEntityVersionPageBlockMapper
    {
        #region constructor

        private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;
        private readonly ILogger<EntityVersionPageBlockMapper> _logger;

        public EntityVersionPageBlockMapper(
            IQueryExecutor queryExecutor,
            IPageVersionBlockModelMapper pageVersionBlockModelMapper,
            ILogger<EntityVersionPageBlockMapper> logger
            )
        {
            _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
            _logger = logger;
        }

        #endregion

        #region private mapping class

        private class MappedPageBlock
        {
            public IEntityVersionPageBlock PageBlock { get; set; }
            public IPageBlockTypeDisplayModel DisplayModel { get; set; }
            public PageBlockTypeSummary BlockType { get; set; }
        }

        #endregion

        public async Task MapRegionsAsync<TBlockRenderDetails>(
            IEnumerable<IEntityVersionPageBlock> dbBlock,
            IEnumerable<IEntityRegionRenderDetails<TBlockRenderDetails>> regions,
            ICollection<PageBlockTypeSummary> allBlockTypes,
            PublishStatusQuery publishStatus,
            IExecutionContext executionContext
            )
            where TBlockRenderDetails : IEntityVersionPageBlockRenderDetails, new()
        {
            var mappedBlocks = await ToBlockMappingDataAsync(dbBlock, allBlockTypes, publishStatus, executionContext);

            // Map Regions

            foreach (var region in regions)
            {
                var regionMappedBlocks = mappedBlocks
                    .Where(m => m.PageBlock.PageTemplateRegionId == region.PageTemplateRegionId)
                    .OrderBy(m => m.PageBlock.Ordering);

                region.Blocks = ToBlockRenderDetails<TBlockRenderDetails>(regionMappedBlocks).ToArray();
            }
        }

        /// <summary>
        /// Locates and returns the correct templates for a block if it a custom template 
        /// assigned, otherwise null is returned.
        /// </summary>
        /// <param name="pageBlock">An unmapped database block to locate the template for.</param>
        /// <param name="blockType">The block type associated with the block in which to look for the template.</param>
        public PageBlockTypeTemplateSummary GetCustomTemplate(IEntityVersionPageBlock pageBlock, PageBlockTypeSummary blockType)
        {
            if (pageBlock == null) throw new ArgumentNullException(nameof(pageBlock));
            if (blockType == null) throw new ArgumentNullException(nameof(blockType));

            if (!pageBlock.PageBlockTypeTemplateId.HasValue) return null;

            var template = blockType
                .Templates
                .FirstOrDefault(t => t.PageBlockTypeTemplateId == pageBlock.PageBlockTypeTemplateId);

            if (template == null)
            {
                _logger.LogDebug("The block template with id {PageBlockTypeTemplateId} could not be found for {PageBlockType} {VersionBlockId}", pageBlock.PageBlockTypeTemplateId, pageBlock.GetType().Name, pageBlock.GetVersionBlockId());
            }

            return template;
        }

        private async Task<ICollection<MappedPageBlock>> ToBlockMappingDataAsync(
            IEnumerable<IEntityVersionPageBlock> entityBlocks, 
            IEnumerable<PageBlockTypeSummary> blockTypes, 
            PublishStatusQuery publishStatus,
            IExecutionContext executionContext
            )
        {
            var mappedBlocks = new List<MappedPageBlock>();

            foreach (var group in entityBlocks.GroupBy(m => m.PageBlockTypeId))
            {
                var blockType = blockTypes.SingleOrDefault(t => t.PageBlockTypeId == group.Key);

                // If missing e.g. archived, skip
                if (blockType == null) continue;

                var mapperOutput = await _pageVersionBlockModelMapper.MapDisplayModelAsync(
                    blockType.FileName, 
                    group, 
                    publishStatus,
                    executionContext
                    );

                foreach (var block in group)
                {
                    var versionBlockId = block.GetVersionBlockId();

                    if (mapperOutput.ContainsKey(versionBlockId) && mapperOutput[versionBlockId] != null)
                    {
                        var mappedBlock = new MappedPageBlock()
                        {
                            PageBlock = block,
                            BlockType = blockType,
                            DisplayModel = mapperOutput[versionBlockId]
                        };

                        mappedBlocks.Add(mappedBlock);
                    }
                }
            }

            return mappedBlocks;
        }

        private IEnumerable<TBlockRenderDetails> ToBlockRenderDetails<TBlockRenderDetails>(IEnumerable<MappedPageBlock> dbBlocks)
            where TBlockRenderDetails : IEntityVersionPageBlockRenderDetails, new()
        {
            int index = 0;
            int size = dbBlocks.Count();

            foreach (var dbBlock in dbBlocks)
            {
                var block = new TBlockRenderDetails();

                block.EntityVersionPageBlockId = dbBlock.PageBlock.GetVersionBlockId();
                block.BlockType = dbBlock.BlockType;
                block.DisplayModel = dbBlock.DisplayModel;
                block.Template = GetCustomTemplate(dbBlock.PageBlock, dbBlock.BlockType);

                // Add any list context information.
                if (block.DisplayModel is IListablePageBlockTypeDisplayModel displayData)
                {
                    displayData.ListContext = new ListablePageBlockRenderContext()
                    {
                        Index = index,
                        NumBlocks = size
                    };

                    index++;
                }

                yield return block;
            }
        }
    }
}
