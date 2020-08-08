using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class ContentRepositoryRewriteRuleExtensions
    {
        /// <summary>
        /// Rewrite rules can be used to redirect users from one url to another.
        /// This functionality is incomplete and subject to change.
        /// </summary>
        public static IAdvancedContentRepositoryRewriteRuleRepository RewriteRules(this IAdvancedContentRepository contentRepository)
        {
            return new ContentRepositoryRewriteRuleRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
