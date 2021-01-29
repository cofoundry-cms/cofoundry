using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    public class GetPageDirectoryNodeByIdQueryHandler 
        : IQueryHandler<GetPageDirectoryNodeByIdQuery, PageDirectoryNode>
        , IPermissionRestrictedQueryHandler<GetPageDirectoryNodeByIdQuery, PageDirectoryNode>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public GetPageDirectoryNodeByIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public async Task<PageDirectoryNode> ExecuteAsync(GetPageDirectoryNodeByIdQuery query, IExecutionContext executionContext)
        {
            var tree = await _queryExecutor.ExecuteAsync(new GetPageDirectoryTreeQuery(), executionContext);
            var result = tree
                .Flatten()
                .SingleOrDefault(n => n.PageDirectoryId == query.PageDirectoryId);

            return result;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageDirectoryNodeByIdQuery command)
        {
            yield return new PageDirectoryReadPermission();
        }

        #endregion
    }
}
