using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetWebDirectoryTreeNodeByIdQueryHandler 
        : IQueryHandler<GetByIdQuery<WebDirectoryNode>, WebDirectoryNode>
        , IAsyncQueryHandler<GetByIdQuery<WebDirectoryNode>, WebDirectoryNode>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<WebDirectoryNode>, WebDirectoryNode>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public GetWebDirectoryTreeNodeByIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public WebDirectoryNode Execute(GetByIdQuery<WebDirectoryNode> query, IExecutionContext executionContext)
        {
            var tree = _queryExecutor.Execute(new GetWebDirectoryTreeQuery());
            var result = tree
                .Flatten()
                .SingleOrDefault(n => n.WebDirectoryId == query.Id);

            return result;
        }

        public async Task<WebDirectoryNode> ExecuteAsync(GetByIdQuery<WebDirectoryNode> query, IExecutionContext executionContext)
        {
            var tree = await _queryExecutor.ExecuteAsync(new GetWebDirectoryTreeQuery());
            var result = tree
                .Flatten()
                .SingleOrDefault(n => n.WebDirectoryId == query.Id);

            return result;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<WebDirectoryNode> command)
        {
            yield return new WebDirectoryReadPermission();
        }

        #endregion
    }
}
