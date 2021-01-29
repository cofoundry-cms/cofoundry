using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IAdvancedContentRespository extension root for the RewriteRule entity.
    /// </summary>
    public interface IAdvancedContentRepositoryRewriteRuleRepository
    {
        #region queries

        /// <summary>
        /// Gets a rewrite rule that matches the specified path in the 
        /// 'WriteFrom' property. If multiple matches are found, the most
        /// recently added rule is returned. Non-file paths are matched with
        /// and without the trailing slash.
        /// </summary>
        /// <param name="path">
        /// Path to check for a rewrite rule. For non-file paths the trailing slash 
        /// is optional. Also supports '*' wildcard matching at the end of the path.
        /// </param>
        IContentRepositoryRewriteRuleByPathQueryBuilder GetByPath(string path);

        /// <summary>
        /// Gets a complete list of all rewrite rules set up in the system.
        /// </summary>
        IContentRepositoryRewriteRuleGetAllQueryBuilder GetAll();

        #endregion

        #region commands

        /// <summary>
        /// Adds a new rewrite rule.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created rewrite rule.</returns>
        Task<int> AddAsync(AddRedirectRuleCommand command);

        #endregion
    }
}
