using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageDirectoryTreeNodeByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<PageDirectoryNode>, PageDirectoryNode>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<PageDirectoryNode>, PageDirectoryNode>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public GetPageDirectoryTreeNodeByIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public async Task<PageDirectoryNode> ExecuteAsync(GetByIdQuery<PageDirectoryNode> query, IExecutionContext executionContext)
        {
            var tree = await _queryExecutor.ExecuteAsync(new GetPageDirectoryTreeQuery());
            var result = tree
                .Flatten()
                .SingleOrDefault(n => n.PageDirectoryId == query.Id);

            return result;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<PageDirectoryNode> command)
        {
            yield return new PageDirectoryReadPermission();
        }

        #endregion
    }
}
