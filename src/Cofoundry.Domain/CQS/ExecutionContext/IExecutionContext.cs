using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// A snapshot of the context in which a Command or Query should be executed.
    /// </summary>
    public interface IExecutionContext
    {
        /// <summary>
        /// The user that the Command/Query should be executed as.
        /// </summary>
        IUserContext UserContext { get; }

        /// <summary>
        /// The datetime that the Commnad/Query has been executed by the user. If the Command execution
        /// is deferred then this date may appear in the past and won't be equivalent to DateTime.UtcNow.
        /// </summary>
        DateTime ExecutionDate { get; }
    }
}
