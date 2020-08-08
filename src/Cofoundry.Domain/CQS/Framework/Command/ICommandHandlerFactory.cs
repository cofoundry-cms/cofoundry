using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS.Internal
{
    /// <summary>
    /// Factory to create ICommandHandler instances. This factory allows you to override
    /// or wrap the existing ICommandHandler implementation
    /// </summary>
    public interface ICommandHandlerFactory
    {
        /// <summary>
        /// Creates a new IAsyncCommandHandler instance with the specified type signature.
        /// </summary>
        IAsyncCommandHandler<T> CreateAsyncHandler<T>() where T : ICommand;
    }
}
