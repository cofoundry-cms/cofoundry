using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for rewrite rules.
/// </summary>
public static class ContentRepositoryRewriteRuleExtensions
{
    extension(IAdvancedContentRepository contentRepository)
    {
        /// <summary>
        /// Rewrite rules can be used to redirect users from one url to another.
        /// This functionality is incomplete and subject to change.
        /// </summary>
        public IAdvancedContentRepositoryRewriteRuleRepository RewriteRules()
        {
            return new ContentRepositoryRewriteRuleRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
