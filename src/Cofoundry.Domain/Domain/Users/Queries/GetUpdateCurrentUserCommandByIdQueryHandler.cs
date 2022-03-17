using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetUpdateCurrentUserCommandByIdQueryHandler
    : IQueryHandler<GetPatchableCommandQuery<UpdateCurrentUserCommand>, UpdateCurrentUserCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;

    public GetUpdateCurrentUserCommandByIdQueryHandler(
        CofoundryDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public async Task<UpdateCurrentUserCommand> ExecuteAsync(GetPatchableCommandQuery<UpdateCurrentUserCommand> query, IExecutionContext executionContext)
    {
        if (!executionContext.UserContext.UserId.HasValue) return null;

        var user = await _dbContext
            .Users
            .AsNoTracking()
            .FilterCanSignIn()
            .FilterById(executionContext.UserContext.UserId.Value)
            .Select(u => new UpdateCurrentUserCommand()
            {
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName
            })
            .SingleOrDefaultAsync();

        return user;
    }
}
