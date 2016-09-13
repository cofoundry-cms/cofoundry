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
    public class GetUserMicroSummaryByEmailQueryHandler 
        : IQueryHandler<GetUserMicroSummaryByEmailQuery, UserMicroSummary>
        , IPermissionRestrictedQueryHandler<GetUserMicroSummaryByEmailQuery, UserMicroSummary>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetUserMicroSummaryByEmailQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public UserMicroSummary Execute(GetUserMicroSummaryByEmailQuery query, IExecutionContext executionContext)
        {
            if (string.IsNullOrWhiteSpace(query.Email))
            {
                return null;
            }

            var user = _dbContext
                .Users
                .AsNoTracking()
                .Where(u => u.Email == query.Email)
                .ProjectTo<UserMicroSummary>()
                .SingleOrDefault();

            return user;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetUserMicroSummaryByEmailQuery query)
        {
            if (query.UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                yield return new CofoundryUserReadPermission();
            }
            else
            {
                yield return new NonCofoundryUserReadPermission();
            }
        }

        #endregion
    }
}
