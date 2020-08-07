using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// An IContentRepository that runs queries and
    /// commands with a system-level elevated user 
    /// account.
    /// </summary>
    public interface IContentRepositoryWithElevatedPermissions 
        : IContentRepository
        , IAdvancedContentRepository
    {
    }
}
