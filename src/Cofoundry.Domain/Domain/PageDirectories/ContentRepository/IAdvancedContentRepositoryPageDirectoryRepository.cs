namespace Cofoundry.Domain;

/// <summary>
/// IAdvancedContentRespository extension root for the PageDirectory entity.
/// </summary>
public interface IAdvancedContentRepositoryPageDirectoryRepository
{
    /// <summary>
    /// Retrieve an image asset by a unique database id.
    /// </summary>
    /// <param name="pageDirectoryId">Id of the directory to get.</param>
    IAdvancedContentRepositoryPageDirectoryByIdQueryBuilder GetById(int pageDirectoryId);

    /// <summary>
    /// Queries that return all page directories.
    /// </summary>
    IAdvancedContentRepositoryPageDirectoryGetAllQueryBuilder GetAll();

    /// <summary>
    /// Query to determine if a page directory UrlPath is unique
    /// within its parent directory.
    /// </summary>
    IDomainRepositoryQueryContext<bool> IsPathUnique(IsPageDirectoryPathUniqueQuery query);

    /// <summary>
    /// Adds a new page directory.
    /// </summary>
    /// <param name="command">Command parameters.</param>
    /// <returns>Id of the newly created directory.</returns>
    Task<int> AddAsync(AddPageDirectoryCommand command);

    /// <summary>
    /// Updates the main properties of an existing page directory. To
    /// update properties that affect the route, use <see cref="UpdateUrlAsync"/>.
    /// </summary>
    /// <param name="command">Command parameters.</param>
    Task UpdateAsync(UpdatePageDirectoryCommand command);

    /// <summary>
    /// Updates the main properties of an existing page directory. To
    /// update properties that affect the route, use <see cref="UpdatePageDirectoryUrlCommand"/>.
    /// </summary>
    /// <param name="pageDirectoryId">
    /// Database id of the page directory to update.
    /// </param>
    /// <param name="commandPatcher">
    /// An action to configure or "patch" a command that's been initialized
    /// with the existing directory data.
    /// </param>
    Task UpdateAsync(int pageDirectoryId, Action<UpdatePageDirectoryCommand> commandPatcher);

    /// <summary>
    /// Updates the url of a page directory. Changing a directory url
    /// will cause the url of any child directories or pages to change. The command
    /// will publish an <see cref="PageDirectoryUrlChangedMessage"/> or <see cref="PageUrlChangedMessage"/>
    /// for any affected directories or pages.
    /// </summary>
    /// <param name="command">Command parameters.</param>
    Task UpdateUrlAsync(UpdatePageDirectoryUrlCommand command);

    /// <summary>
    /// Removes a page directory from the system. The root directory cannot
    /// be deleted.
    /// </summary>
    /// <param name="pageDirectoryId">Id of the page directory to delete.</param>
    Task DeleteAsync(int pageDirectoryId);

    /// <summary>
    /// <para>
    /// Access rules are used to restrict access to a website resource to users
    /// fulfilling certain criteria such as a specific user area or role. Page
    /// directory access rules are used to define the rules at a <see cref="PageDirectory"/> 
    /// level. These rules are inherited by child directories and pages.
    /// </para>
    /// <para>
    /// Note that access rules do not apply to users from the Cofoundry Admin user
    /// area. They aren't intended to be used to restrict editor access in the admin UI 
    /// but instead are used to restrict public access to website pages and routes.
    /// </para>
    /// </summary>
    IAdvancedContentRepositoryPageDirectoryAccessRulesRepository AccessRules();
}
