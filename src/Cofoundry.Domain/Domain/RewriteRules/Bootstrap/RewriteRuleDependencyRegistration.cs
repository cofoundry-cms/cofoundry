using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class RewriteRuleDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<IRewriteRuleCache, RewriteRuleCache>()
            .Register<IRewriteRuleSummaryMapper, RewriteRuleSummaryMapper>()
            ;
    }
}
