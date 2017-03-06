using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Indicates that the execution of an ICommand should be logged.
    /// </summary>
    public interface ILoggableCommand : ICommand
    {
    }
}
