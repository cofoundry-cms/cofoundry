using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetAllPageTemplateMicroSummariesQueryHandler
    : IQueryHandler<GetAllPageTemplateMicroSummariesQuery, ICollection<PageTemplateMicroSummary>>
    , IPermissionRestrictedQueryHandler<GetAllPageTemplateMicroSummariesQuery, ICollection<PageTemplateMicroSummary>>
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

    public async Task<ICollection<PageTemplateMicroSummary>> ExecuteAsync(GetAllPageTemplateMicroSummariesQuery query, IExecutionContext executionContext)
    {
        var dbResults = await Query().ToListAsync();
        var results = dbResults
            .Select(_pageTemplateMapper.Map)
            .ToList();

        return results;
    }

    private IQueryable<PageTemplate> Query()
    {
        return _dbContext
            .PageTemplates
            .AsNoTracking()
            .FilterActive()
            .OrderBy(l => l.FileName);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetAllPageTemplateMicroSummariesQuery query)
    {
        yield return new PageTemplateReadPermission();
    }
}