using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.BackgroundTasks
{
    /// <summary>
    /// Represents a task to execute 
    /// </summary>
    public interface IBackgroundTask
    {
        void Execute();
    }
}
