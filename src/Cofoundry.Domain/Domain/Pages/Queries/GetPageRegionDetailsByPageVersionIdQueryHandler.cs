using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns a collection of content managed regions with
    /// block data for a specific version of a page.
    /// </summary>
    public class GetPageRegionDetailsByPageVersionIdQueryHandler 
        : IQueryHandler<GetPageRegionDetailsByPageVersionIdQuery, ICollection<PageRegionDetails>>
        , IPermissionRestrictedQueryHandler<GetPageRegionDetailsByPageVersionIdQuery, ICollection<PageRegionDetails>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageVersionBlockModelMapper _pageVersionBlockModelMapper;
        private readonly IEntityVersionPageBlockMapper _entityVersionPageBlockMapper;
        
        public GetPageRegionDetailsByPageVersionIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageVersionBlockModelMapper pageVersionBlockModelMapper,
            IEntityVersionPageBlockMapper entityVersionPageBlockMapper
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageVersionBlockModelMapper = pageVersionBlockModelMapper;
            _entityVersionPageBlockMapper = entityVersionPageBlockMapper;
        }

        #endregion

        #region execution

        public async Task<ICollection<PageRegionDetails>> ExecuteAsync(GetPageRegionDetailsByPageVersionIdQuery query, IExecutionContext executionContext)
        {
            var regions = await GetRegions(query).ToListAsync();
            var dbPageBlocks = await QueryPageBlocks(query).ToListAsync();
            var allBlockTypes = await _queryExecutor.ExecuteAsync(new GetAllPageBlockTypeSummariesQuery(), executionContext);

            MapRegions(regions, dbPageBlocks, allBlockTypes);

            return regions;
        }

        private void MapRegions(
            List<PageRegionDetails> regions, 
            List<PageVersionBlock> dbPageBlocks,
            ICollection<PageBlockTypeSummary> allBlockTypes
            )
        {
            foreach (var region in regions)
            {
                var regionMappedBlocks = dbPageBlocks
                    .Where(m => m.PageTemplateRegionId == region.PageTemplateRegionId)
                    .OrderBy(m => m.Ordering)
                    .Select(m => MapPageBlock(m, allBlockTypes));

                region.Blocks = regionMappedBlocks.ToArray();
            }
        }

        private IQueryable<PageVersionBlock> QueryPageBlocks(GetPageRegionDetailsByPageVersionIdQuery query)
        {
            return _dbContext
                .PageVersionBlocks
                .AsNoTracking()
                .FilterActive()
                .Where(m => m.PageVersionId == query.PageVersionId);
        }

        private PageVersionBlockDetails MapPageBlock(PageVersionBlock dbBlock, ICollection<PageBlockTypeSummary> allBlockTypes)
        {
            var blockType = allBlockTypes.SingleOrDefault(t => t.PageBlockTypeId == dbBlock.PageBlockTypeId);

            var block = new PageVersionBlockDetails();
            block.BlockType = blockType;
            block.DataModel = _pageVersionBlockModelMapper.MapDataModel(blockType.FileName, dbBlock);
            block.PageVersionBlockId = dbBlock.PageVersionBlockId;
            block.Template = _entityVersionPageBlockMapper.GetCustomTemplate(dbBlock, blockType);

            return block;
        }

        private IQueryable<PageRegionDetails> GetRegions(GetPageRegionDetailsByPageVersionIdQuery query)
        {
            var dbQuery = _dbContext
                .PageVersions
                .AsNoTracking()
                .FilterActive()
                .FilterByPageVersionId(query.PageVersionId)
                .SelectMany(v => v.PageTemplate.PageTemplateRegions)
                .Where(s => !s.IsCustomEntityRegion)
                .OrderBy(s => s.UpdateDate)
                .Select(s => new PageRegionDetails()
                {
                    PageTemplateRegionId = s.PageTemplateRegionId,
                    Name = s.Name
                });

            return dbQuery;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRegionDetailsByPageVersionIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
