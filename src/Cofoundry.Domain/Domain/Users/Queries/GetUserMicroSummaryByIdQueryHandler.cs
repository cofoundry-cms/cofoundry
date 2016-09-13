using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;

namespace Cofoundry.Domain
{
    public class GetUserMicroSummaryByIdQueryHandler 
        : IQueryHandler<GetByIdQuery<UserMicroSummary>, UserMicroSummary>
        , IIgnorePermissionCheckHandler
    {
        #region constructor
        
        private readonly CofoundryDbContext _dbContext;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetUserMicroSummaryByIdQueryHandler(
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public UserMicroSummary Execute(GetByIdQuery<UserMicroSummary> query, IExecutionContext executionContext)
        {
            var user = _dbContext
                .Users
                .AsNoTracking()
                .Where(u => u.UserId == query.Id)
                .ProjectTo<UserMicroSummary>()
                .SingleOrDefault();

            if (user != null && user.UserArea.UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                _permissionValidationService.EnforceCurrentUserOrHasPermission<CofoundryUserReadPermission>(query.Id, executionContext.UserContext);
            }
            else if (user != null)
            {
                _permissionValidationService.EnforceCurrentUserOrHasPermission<NonCofoundryUserReadPermission>(query.Id, executionContext.UserContext);
            }

            return user;
        }

        #endregion
    }
}
