using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Factory for creating an IExecutionContext instance
    /// </summary>
    public interface IExecutionContextFactory
    {
        IExecutionContext Create();
        IExecutionContext Create(IUserContext userContext);
    }
}
