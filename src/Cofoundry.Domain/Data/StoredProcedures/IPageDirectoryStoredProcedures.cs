namespace Cofoundry.Domain.Data.Internal;

/// <inheritdoc/>
public interface IPageDirectoryStoredProcedures
{
    /// <summary>
    /// <para>
    /// Regenerates the PageDirectoryClosure table, upserting any 
    /// missing or out of date values. After the update is completed
    /// the PageDirectoryPath table is also updated, because the path
    /// data is dependent on the data in the closure table.
    /// </para>
    /// <para>
    /// This procedure should be called whenever page directories
    /// are added, or the heirachy is updated by changing 
    /// ParentDirectoryIds. Although the procedure does handle
    /// deletions, it's not necessary to run it for deletions as this 
    /// is also handled by the page directory cascade 
    /// delete trigger. 
    /// </para>
    /// </summary>
    Task UpdatePageDirectoryClosureAsync();

    /// <summary>
    /// <para>
    /// Regenerates the PageDirectoryPath table, upserting any missing
    /// or out of date values. This only needs to be called when the UrlPath
    /// of a page directory changes because in most other scenarios either the
    /// <see cref="UpdatePageDirectoryClosureAsync"/> method makes the same 
    /// changes or for deletions the cascade delete trigger handles any necessary
    /// deletions.
    /// </para>
    /// </summary>
    Task UpdatePageDirectoryPathAsync();
}
