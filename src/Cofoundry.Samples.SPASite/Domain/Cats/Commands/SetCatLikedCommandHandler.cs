using Cofoundry.Core.EntityFramework;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Samples.SPASite.Data;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    /// <summary>
    /// This handler uses ILoggedInPermissionCheckHandler to make sure
    /// a user logged in before allowing them to set the cat as liked.
    /// We could use IPermissionRestrictedCommandHandler to be more 
    /// specific here and create a specific permission for the action,
    /// but that isn't neccessary here because any logged in user
    /// can perform this action.
    /// 
    /// For more on permissions see https://www.cofoundry.org/docs/framework/roles-and-permissions
    /// </summary>
    public class SetCatLikedCommandHandler
        : ICommandHandler<SetCatLikedCommand>
        , ILoggedInPermissionCheckHandler
    {
        private readonly IEntityFrameworkSqlExecutor _entityFrameworkSqlExecutor;
        private readonly SPASiteDbContext _spaSiteDbContext;

        public SetCatLikedCommandHandler(
            IEntityFrameworkSqlExecutor entityFrameworkSqlExecutor,
            SPASiteDbContext spaSiteDbContext
            )
        {
            _entityFrameworkSqlExecutor = entityFrameworkSqlExecutor;
            _spaSiteDbContext = spaSiteDbContext;
        }

        public Task ExecuteAsync(SetCatLikedCommand command, IExecutionContext executionContext)
        {
            // We could use the EF DbContext here, but it's faster to make this change using a 
            // stored procedure. We use IEntityFrameworkSqlExecutor here to simplify this.
            // For more info see https://www.cofoundry.org/docs/framework/entity-framework-and-dbcontext-tools#executing-stored-procedures--raw-sql

            return _entityFrameworkSqlExecutor
                .ExecuteCommandAsync(_spaSiteDbContext,
                "app.CatLike_SetLiked",
                 new SqlParameter("@CatId", command.CatId),
                 new SqlParameter("@UserId", executionContext.UserContext.UserId),
                 new SqlParameter("@IsLiked", command.IsLiked),
                 new SqlParameter("@CreateDate", executionContext.ExecutionDate)
                 );
        }
    }

}
