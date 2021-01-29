using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple access to Cofoundry domain queries and commands from 
    /// a single repository. A more extensive range of queries and
    /// commands are available in IAdvancedContentRepository.
    /// </summary>
    public interface IContentRepository : IDomainRepository
    {
    }
}
