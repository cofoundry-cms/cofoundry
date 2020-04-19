using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple facade over rewrite rule data access queries/commands to them more discoverable
    /// in implementations.
    /// </summary>
    [Obsolete("Use the new IContentRepository instead.")]
    public class RewriteRuleRepository : IRewriteRuleRepository
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public RewriteRuleRepository(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        #endregion

        #region queries

        public Task<ICollection<RewriteRuleSummary>> GetAllRewriteRuleSummariesAsync(IExecutionContext executionContext = null)
        {
            var query = new GetAllRewriteRuleSummariesQuery();
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        /// <summary>
        /// Gets a rewrite rule that matches the specified path in the 
        /// 'WriteFrom' property. If multiple matches are found, the most
        /// recently added rule is returned. Non-file paths are matched with
        /// and without the trailing slash.
        /// </summary>
        /// <param name="path">
        /// Path to check for a rewrite rule. for non-file paths the trailing slash 
        /// is optional. Also supports '*' wildcard matching at the end of the path.
        /// </param>
        public Task<RewriteRuleSummary> GetRewriteRuleByPathAsync(string path, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetRewriteRuleSummaryByPathQuery() { Path = path }, executionContext);
        }

        #endregion

        #region commands

        public async Task<int> AddRedirectRuleAsync(AddRedirectRuleCommand command, IExecutionContext executionContext = null)
        {
            await _commandExecutor.ExecuteAsync(command);

            return command.OutputRedirectRuleId;
        }

        #endregion
    }
}
