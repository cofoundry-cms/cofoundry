using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A mapping helper containing a couple of mapping methods used in multiple queires
    /// to map page blocks in regular pages as well as custom entity details pages.
    /// </summary>
    public class EntityVersionPageBlockMapper : IEntityVersionPageBlockMapper
    {
        #region constructor

        private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;

        public EntityVersionPageBlockMapper(
            IQueryExecutor queryExecutor,
            IPageVersionBlockModelMapper pageVersionBlockModelMapper
            )
        {
            _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
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

        #region public methods

        public async Task MapRegionsAsync<TBlockRenderDetails>(
            IEnumerable<IEntityVersionPageBlock> dbBlock,
            IEnumerable<IEntityRegionRenderDetails<TBlockRenderDetails>> regions,
            IEnumerable<PageBlockTypeSummary> allBlockTypes,
            PublishStatusQuery publishStatus
            )
            where TBlockRenderDetails : IEntityVersionPageBlockRenderDetails, new()
        {
            var mappedBlocks = await ToBlockMappingDataAsync(dbBlock, allBlockTypes, publishStatus);

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

            Debug.Assert(template != null, string.Format("The block template with id {0} could not be found for {1} {2}", pageBlock.PageBlockTypeTemplateId, pageBlock.GetType().Name, pageBlock.GetVersionBlockId()));

            return template;
        }


        #endregion

        #region private methods

        private async Task<List<MappedPageBlock>> ToBlockMappingDataAsync(
            IEnumerable<IEntityVersionPageBlock> entityBlocks, 
            IEnumerable<PageBlockTypeSummary> blockTypes, 
            PublishStatusQuery workflowStatus
            )
        {
            var mappedBlocks = new List<MappedPageBlock>();

            foreach (var group in entityBlocks.GroupBy(m => m.PageBlockTypeId))
            {
                var blockType = blockTypes.SingleOrDefault(t => t.PageBlockTypeId == group.Key);
                var mapperOutput = await _pageVersionBlockModelMapper.MapDisplayModelAsync(blockType.FileName, group, workflowStatus);

                foreach (var block in group)
                {
                    var mappedBlock = new MappedPageBlock()
                    {
                        PageBlock = block,
                        BlockType = blockType,
                        DisplayModel = mapperOutput.Single(o => o.VersionBlockId == block.GetVersionBlockId()).DisplayModel
                    };

                    mappedBlocks.Add(mappedBlock);
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
                var displayData = block.DisplayModel as IListablePageBlockTypeDisplayModel;

                if (displayData != null)
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

        #endregion
    }
}
