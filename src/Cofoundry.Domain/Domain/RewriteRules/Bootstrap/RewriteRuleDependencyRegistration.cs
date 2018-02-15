using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain.Bootstrap
{
    public class RewriteRuleDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<IRewriteRuleRepository, RewriteRuleRepository>()
                .Register<IRewriteRuleCache, RewriteRuleCache>()
                .Register<IRewriteRuleSummaryMapper, RewriteRuleSummaryMapper>()
                ;
        }
    }
}
