using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets all page block types registered in the system. The results of this query
    /// are cached by default.
    /// </summary>
    public class GetAllPageBlockTypeSummariesQuery : IQuery<ICollection<PageBlockTypeSummary>>
    {
    }
}
