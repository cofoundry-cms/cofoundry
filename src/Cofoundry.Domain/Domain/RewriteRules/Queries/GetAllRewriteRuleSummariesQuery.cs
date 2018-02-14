using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetAllRewriteRuleSummariesQuery : IQuery<ICollection<RewriteRuleSummary>>
    {
    }
}
