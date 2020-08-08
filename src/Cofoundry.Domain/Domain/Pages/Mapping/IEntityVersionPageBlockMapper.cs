using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// A mapping helper containing a couple of mapping methods used in multiple queires
    /// to map page blocks in regular pages as well as custom entity details pages.
    /// </summary>
    public interface IEntityVersionPageBlockMapper
    {
        Task MapRegionsAsync<TBlockRenderDetails>(
            IEnumerable<IEntityVersionPageBlock> dbBlocks,
            IEnumerable<IEntityRegionRenderDetails<TBlockRenderDetails>> regions,
            ICollection<PageBlockTypeSummary> allBlockTypes,
            PublishStatusQuery publishStatus,
            IExecutionContext executionContext
            )
            where TBlockRenderDetails : IEntityVersionPageBlockRenderDetails, new();

        /// <summary>
        /// Locates and returns the correct templates for a block if it a custom template 
        /// assigned, otherwise null is returned.
        /// </summary>
        /// <param name="pageBlock">An unmapped database block to locate the template for.</param>
        /// <param name="blockType">The block type associated with the block in which to look for the template.</param>
        PageBlockTypeTemplateSummary GetCustomTemplate(IEntityVersionPageBlock pageBlock, PageBlockTypeSummary blockType);
    }
}
