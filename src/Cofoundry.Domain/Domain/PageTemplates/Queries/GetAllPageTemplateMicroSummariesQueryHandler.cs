using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetAllPageTemplateMicroSummariesQueryHandler
    : IQueryHandler<GetAllPageTemplateMicroSummariesQuery, IReadOnlyCollection<PageTemplateMicroSummary>>
    , IPermissionRestrictedQueryHandler<GetAllPageTemplateMicroSummariesQuery, IReadOnlyCollection<PageTemplateMicroSummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageTemplateMicroSummaryMapper _pageTemplateMapper;

    public GetAllPageTemplateMicroSummariesQueryHandler(
        CofoundryDbContext dbContext,
        IPageTemplateMicroSummaryMapper pageTemplateMapper
        )
    {
        _dbContext = dbContext;
        _pageTemplateMapper = pageTemplateMapper;
    }

    public async Task<IReadOnlyCollection<PageTemplateMicroSummary>> ExecuteAsync(GetAllPageTemplateMicroSummariesQuery query, IExecutionContext executionContext)
    {
        var dbResults = await _dbContext
            .PageTemplates
            .AsNoTracking()
            .FilterActive()
            .OrderBy(l => l.FileName)
            .ToArrayAsync();

        var results = dbResults
            .Select(_pageTemplateMapper.Map)
            .ToArray();

        return results;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetAllPageTemplateMicroSummariesQuery query)
    {
        yield return new PageTemplateReadPermission();
    }
}