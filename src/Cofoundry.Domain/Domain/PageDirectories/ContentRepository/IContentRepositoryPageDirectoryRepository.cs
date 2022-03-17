namespace Cofoundry.Domain;

/// <summary>
/// IContentRespository extension root for the ImageAsset entity.
/// </summary>
public interface IContentRepositoryPageDirectoryRepository
{
    /// <summary>
    /// Query a single page directory by it's database id.
    /// </summary>
    /// <param name="pageDirectoryId">Id of the directory to get.</param>
    IContentRepositoryPageDirectoryByIdQueryBuilder GetById(int pageDirectoryId);

    /// <summary>
    /// Queries that return all page directories.
    /// </summary>
    IContentRepositoryPageDirectoryGetAllQueryBuilder GetAll();
}