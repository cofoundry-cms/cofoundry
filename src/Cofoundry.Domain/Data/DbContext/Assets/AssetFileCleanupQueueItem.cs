using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// This queue keeps track of files belonging to assets that 
    /// have been deleted in the system. The files don't get deleted
    /// at the same time as the asset record and instead are queued
    /// and deleted by a background task to avoid issues with file
    /// locking and other errors that may cause the delete operation 
    /// to fail. It also enabled the file deletion to run in a 
    /// transaction.
    /// </summary>
    public partial class AssetFileCleanupQueueItem
    {
        /// <summary>
        /// Database id of the queue item.
        /// </summary>
        public int AssetFileCleanupQueueItemId { get; set; }

        /// <summary>
        /// The 6 character definition code of the asset entity i.e. 
        /// the image or document asset entity definition code.
        /// </summary>
        public string EntityDefinitionCode { get; set; }

        /// <summary>
        /// The filename of the asset to delete without the file 
        /// extension. Copied directly from the deleted asset record.
        /// </summary>
        public string FileNameOnDisk { get; set; }

        /// <summary>
        /// The file extension of the asset to delete without the 
        /// leading dot. Copied directly from the deleted asset record.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Date that the queue record was created i.e. the date the
        /// asset record was deleted.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The date at which the queue item was last attempted.
        /// </summary>
        public DateTime? LastAttemptDate { get; set; }

        /// <summary>
        /// The date the file deletion operation was completed.
        /// </summary>
        public DateTime? CompletedDate { get; set; }

        /// <summary>
        /// If the file is not deleted after a reasonable number of
        /// attempts then this flag is set, which prevents it being
        /// attempted again.
        /// </summary>
        public bool CanRetry { get; set; }

        /// <summary>
        /// The queue system supports an incremental re-try policy and this
        /// property tracks the date at which the deletion task can be attempted
        /// again.
        /// </summary>
        public DateTime AttemptPermittedDate { get; set; }
    }
}
