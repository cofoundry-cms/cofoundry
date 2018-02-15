using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// A view helper for providing information about the currently logged in user
    /// </summary>
    public class CurrentUserViewHelper : ICurrentUserViewHelper
    {
        #region construction

        private CurrentUserViewHelperContext _context = null;

        private readonly IUserContextService _userContextServiceService;
        private readonly IQueryExecutor _queryExecutor;

        public CurrentUserViewHelper(
            IUserContextService userContextServiceService,
            IQueryExecutor queryExecutor
            )
        {
            _userContextServiceService = userContextServiceService;
            _queryExecutor = queryExecutor;
        }

        #endregion

        public async Task<CurrentUserViewHelperContext> GetAsync()
        {
            // since this only runs in views it shouldn't need to be threadsafe
            if (_context == null)
            {
                await InitializeContextAsync();
            }

            return _context;
        }

        private async Task InitializeContextAsync()
        {
            var context = new CurrentUserViewHelperContext();
            var userContext = await _userContextServiceService.GetCurrentContextAsync();
            context.Role = await _queryExecutor.ExecuteAsync(new GetRoleDetailsByIdQuery(userContext.RoleId));

            if (userContext.UserId.HasValue)
            {
                var query = new GetUserMicroSummaryByIdQuery(userContext.UserId.Value);
                context.User = await _queryExecutor.ExecuteAsync(query);
                context.IsLoggedIn = true;
            }

            _context = context;
        }
    }
}
