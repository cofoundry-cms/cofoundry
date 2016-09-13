using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class DoesPageHaveDraftVersionQueryHandler
        : IQueryHandler<DoesPageHaveDraftVersionQuery, bool>
        , IPermissionRestrictedQueryHandler<DoesPageHaveDraftVersionQuery, bool>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public DoesPageHaveDraftVersionQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public bool Execute(DoesPageHaveDraftVersionQuery query, IExecutionContext executionContext)
        {
            var exists = _dbContext
                .PageVersions
                .Any(v => v.PageId == query.PageId && v.WorkFlowStatusId == (int)WorkFlowStatus.Draft && !v.IsDeleted);

            return exists;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DoesPageHaveDraftVersionQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }

}
