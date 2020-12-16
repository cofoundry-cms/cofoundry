using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A single repository for all Cofoundry queries and
    /// commands to make them easier to discover.
    /// </summary>
    public interface IAdvancedContentRepository : IDomainRepository
    {
    }
}
