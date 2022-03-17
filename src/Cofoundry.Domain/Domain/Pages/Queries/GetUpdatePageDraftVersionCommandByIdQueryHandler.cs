using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetUpdatePageDraftVersionCommandByIdQueryHandler
    : IQueryHandler<GetPatchableCommandByIdQuery<UpdatePageDraftVersionCommand>, UpdatePageDraftVersionCommand>
    , IPermissionRestrictedQueryHandler<GetPatchableCommandByIdQuery<UpdatePageDraftVersionCommand>, UpdatePageDraftVersionCommand>
{
    private readonly CofoundryDbContext _dbContext;

    public GetUpdatePageDraftVersionCommandByIdQueryHandler(
        CofoundryDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public async Task<UpdatePageDraftVersionCommand> ExecuteAsync(GetPatchableCommandByIdQuery<UpdatePageDraftVersionCommand> query, IExecutionContext executionContext)
    {
        var command = await _dbContext
            .PageVersions
            .AsNoTracking()
            .FilterActive()
            .FilterByPageId(query.Id)
            .Where(p => p.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
            .Select(v => new UpdatePageDraftVersionCommand
            {
                MetaDescription = v.MetaDescription,
                OpenGraphDescription = v.OpenGraphDescription,
                OpenGraphImageId = v.OpenGraphImageId,
                OpenGraphTitle = v.OpenGraphTitle,
                PageId = v.PageId,
                ShowInSiteMap = !v.ExcludeFromSitemap,
                Title = v.Title
            })
            .SingleOrDefaultAsync();

        return command;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPatchableCommandByIdQuery<UpdatePageDraftVersionCommand> query)
    {
        yield return new PageReadPermission();
    }
}
