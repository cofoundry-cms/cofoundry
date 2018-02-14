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

        private readonly IUserContextService _userContextServiceService;
        private readonly IQueryExecutor _queryExecutor;

        private IUserContext _userContext = null;
        private UserMicroSummary _user = null;
        private RoleDetails _role = null;
        private bool isInitialized = false;

        public CurrentUserViewHelper(
            IUserContextService userContextServiceService,
            IQueryExecutor queryExecutor
            )
        {
            _userContextServiceService = userContextServiceService;
            _queryExecutor = queryExecutor;
        }

        #endregion

        public async Task EnsureInitializedAsync()
        {
            _userContext = await _userContextServiceService.GetCurrentContextAsync();
            _role = await _queryExecutor.ExecuteAsync(new GetRoleDetailsByIdQuery(_userContext.RoleId));

            if (_userContext.UserId.HasValue)
            {
                var query = new GetUserMicroSummaryByIdQuery(_userContext.UserId.Value);
                _user = await _queryExecutor.ExecuteAsync(query);
            }

            isInitialized = true;
        }

        /// <summary>
        /// Indicates whether the user is logged in
        /// </summary>
        public bool IsLoggedIn
        {
            get
            {
                return Context.UserId.HasValue;
            }
        }

        /// <summary>
        /// Indicates whether the user is logged in and is a user of the Cofoundry admin area. The user
        /// table may be used by non-Cofoundry users too so this differentiates them.
        /// </summary>
        public bool IsCofoundryUser
        {
            get
            {
                return Context.IsCofoundryUser();
            }
        }

        /// <summary>
        /// The context of the currently logged in user
        /// </summary>
        public IUserContext Context
        {
            get
            {
                if (_userContext == null)
                {
                    throw new InvalidOperationException("You must call EnsureInitializedAsync() before accessing properties on the CurrentUserViewHelper.");
                }
                return _userContext;
            }
        }

        /// <summary>
        /// Information about the currently logged in user.
        /// </summary>
        public UserMicroSummary User
        {
            get
            {
                if (!isInitialized)
                {
                    throw new InvalidOperationException("You must call EnsureInitializedAsync() before accessing properties on the CurrentUserViewHelper.");
                }
                return _user;
            }
        }

        /// <summary>
        /// Information about the currently logged in user.
        /// </summary>
        public RoleDetails Role
        {
            get
            {
                if (!isInitialized)
                {
                    throw new InvalidOperationException("You must call EnsureInitializedAsync() before accessing properties on the CurrentUserViewHelper.");
                }
                return _role;
            }
        }
    }
}
