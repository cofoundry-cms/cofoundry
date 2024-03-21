﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Searches image assets based on simple filter criteria and 
/// returns a paged set of summary results. 
/// </summary>
public class SearchImageAssetSummariesQueryHandler
    : IQueryHandler<SearchImageAssetSummariesQuery, PagedQueryResult<ImageAssetSummary>>
    , IPermissionRestrictedQueryHandler<SearchImageAssetSummariesQuery, PagedQueryResult<ImageAssetSummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IImageAssetSummaryMapper _imageAssetSummaryMapper;

    public SearchImageAssetSummariesQueryHandler(
        CofoundryDbContext dbContext,
        IImageAssetSummaryMapper imageAssetSummaryMapper
        )
    {
        _dbContext = dbContext;
        _imageAssetSummaryMapper = imageAssetSummaryMapper;
    }

    public async Task<PagedQueryResult<ImageAssetSummary>> ExecuteAsync(SearchImageAssetSummariesQuery query, IExecutionContext executionContext)
    {
        IQueryable<ImageAsset> dbQuery = _dbContext
            .ImageAssets
            .AsNoTracking()
            .Include(i => i.Creator)
            .Include(i => i.Updater)
            .Include(i => i.ImageAssetTags)
            .ThenInclude(i => i.Tag);

        // Filter by tags
        if (!string.IsNullOrEmpty(query.Tags))
        {
            var tags = TagParser.Split(query.Tags).ToList();
            foreach (var tag in tags)
            {
                // See http://stackoverflow.com/a/7288269/486434 for why this is copied into a new variable
                var localTag = tag;

                dbQuery = dbQuery.Where(p => p.ImageAssetTags
                    .Select(t => t.Tag.TagText)
                    .Contains(localTag)
                    );
            }
        }

        // Filter Dimensions
        if (query.Height > 0)
        {
            dbQuery = dbQuery.Where(p => p.HeightInPixels == query.Height);
        }
        else if (query.MinHeight > 0)
        {
            dbQuery = dbQuery.Where(p => p.HeightInPixels >= query.MinHeight);
        }

        if (query.Width > 0)
        {
            dbQuery = dbQuery.Where(p => p.WidthInPixels == query.Width);
        }
        else if (query.MinWidth > 0)
        {
            dbQuery = dbQuery.Where(p => p.WidthInPixels >= query.MinWidth);
        }

        var dbPagedResults = await dbQuery
            .OrderByDescending(p => p.CreateDate)
            .ToPagedResultAsync(query);

        var mappedResults = dbPagedResults
            .Items
            .Select(_imageAssetSummaryMapper.Map)
            .ToArray();

        return dbPagedResults.ChangeType(mappedResults);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(SearchImageAssetSummariesQuery query)
    {
        yield return new ImageAssetReadPermission();
    }
}