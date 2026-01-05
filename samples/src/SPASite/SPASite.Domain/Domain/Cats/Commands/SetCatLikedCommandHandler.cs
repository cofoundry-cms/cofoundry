using Cofoundry.Core.EntityFramework;
using Microsoft.Data.SqlClient;
using SPASite.Data;

namespace SPASite.Domain;

/// <summary>
/// This handler uses <see cref="ISignedInPermissionCheckHandler"/>
/// to make sure a user signed in before allowing them to set the cat
/// as liked. We could use <see cref="IPermissionRestrictedCommandHandler"/>
/// to be more specific here and create a specific permission for the action,
/// but that isn't neccessary here because any signed in user
/// can perform this action.
/// 
/// For more on permissions see https://www.cofoundry.org/docs/framework/roles-and-permissions
/// </summary>
public class SetCatLikedCommandHandler
    : ICommandHandler<SetCatLikedCommand>
    , ISignedInPermissionCheckHandler
{
    private readonly IEntityFrameworkSqlExecutor _entityFrameworkSqlExecutor;
    private readonly SPASiteDbContext _spaSiteDbContext;

    public SetCatLikedCommandHandler(
        IEntityFrameworkSqlExecutor entityFrameworkSqlExecutor,
        SPASiteDbContext spaSiteDbContext)
    {
        _entityFrameworkSqlExecutor = entityFrameworkSqlExecutor;
        _spaSiteDbContext = spaSiteDbContext;
    }

    public Task ExecuteAsync(SetCatLikedCommand command, IExecutionContext executionContext)
    {
        // We could use the EF DbContext here, but it's faster to make this change using a 
        // stored procedure. We use IEntityFrameworkSqlExecutor here to simplify this, but
        // you could also use EF directly, Dapper or mix in any other data access approach.
        // For more info see https://www.cofoundry.org/docs/framework/entity-framework-and-dbcontext-tools#executing-stored-procedures--raw-sql

        return _entityFrameworkSqlExecutor.ExecuteCommandAsync(
            _spaSiteDbContext,
            "app.CatLike_SetLiked",
            new SqlParameter("@CatId", command.CatId),
            new SqlParameter("@UserId", executionContext.UserContext.UserId),
            new SqlParameter("@IsLiked", command.IsLiked),
            new SqlParameter("@CreateDate", executionContext.ExecutionDate));
    }
}

