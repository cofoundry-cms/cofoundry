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

        private Lazy<IUserContext> _userContext;
        private Lazy<UserMicroSummary> _user;
        private Lazy<RoleDetails> _role;
         
        public CurrentUserViewHelper(
            IUserContextService userContextServiceService,
            IQueryExecutor queryExecutor
            )
        {
            _userContextServiceService = userContextServiceService;
            _queryExecutor = queryExecutor;

            _userContext = new Lazy<IUserContext>(InitUserContext);
            _user = new Lazy<UserMicroSummary>(InitUser);
            _role = new Lazy<RoleDetails>(InitRole);
        }

        private IUserContext InitUserContext()
        {
            return _userContextServiceService.GetCurrentContext();
        }

        private UserMicroSummary InitUser()
        {
            if (!Context.UserId.HasValue) return null;

            return _queryExecutor.GetById<UserMicroSummary>(Context.UserId.Value);
        }

        private RoleDetails InitRole()
        {
            return _queryExecutor.Execute(new GetRoleDetailsByIdQuery(Context.RoleId));
        }

        #endregion

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
                return _userContext.Value;
            }
        }

        /// <summary>
        /// Information about the currently logged in user.
        /// </summary>
        public UserMicroSummary User
        {
            get
            {
                return _user.Value;
            }
        }

        /// <summary>
        /// Information about the currently logged in user.
        /// </summary>
        public RoleDetails Role
        {
            get
            {
                return _role.Value;
            }
        }
    }
}
