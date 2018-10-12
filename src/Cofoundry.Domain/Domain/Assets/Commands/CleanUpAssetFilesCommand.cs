using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// Processes a batch of items in the asset file cleanup 
    /// queue, deleting any asset files associated with the queue
    /// items. If any errors occur then the item is re-queued to
    /// try again later.
    /// </para>
    /// <para>
    /// The process is run inside a distributed lock to prevent
    /// the process running concurrently.
    /// </para>
    /// </summary>
    public class CleanUpAssetFilesCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// The number of asset files to process in this batch.
        /// </summary>
        public int BatchSize { get; set; } = 60;
    }
}
