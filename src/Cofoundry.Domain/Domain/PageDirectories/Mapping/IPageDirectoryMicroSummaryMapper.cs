using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// A mapper for mapping <see cref="PageDirectoryMicroSummary"/> projections.
/// </summary>
public interface IPageDirectoryMicroSummaryMapper
{
    /// <summary>
    /// <para>
    /// Maps a <see cref="PageDirectoryMicroSummary"/> projection from a 
    /// <see cref="PageDirectory"/> record. If <paramref name="pageDirectory"/>
    /// is null then <see langword="null"/> is returned.
    /// </para>
    /// </summary>
    /// <param name="pageDirectory">
    /// The database record to map from. This must include the 
    /// <see cref="PageDirectory.PageDirectoryPath"/> relation in the
    /// Entity Framework query.
    /// </param>
    /// <returns>
    /// Mapped projection, or <see langword="null"/> if <paramref name="pageDirectory"/> is <see langword="null"/>.
    /// </returns>
    PageDirectoryMicroSummary Map(PageDirectory pageDirectory);
}
