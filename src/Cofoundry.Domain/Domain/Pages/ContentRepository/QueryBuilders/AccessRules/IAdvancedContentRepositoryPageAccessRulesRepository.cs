namespace Cofoundry.Domain;

/// <summary>
/// Queries and commands for page access rules.
/// </summary>
public interface IAdvancedContentRepositoryPageAccessRulesRepository
{
    /// <summary>
    /// Retrieve page access configuration data by the id of the parent page.
    /// </summary>
    /// <param name="pageId">PageId of the page to get access configuration data for.</param>
    IAdvancedContentRepositoryPageAccessByPageIdQueryBuilder GetByPageId(int pageId);

    /// <summary>
    /// Updates all access rules associated with a page.
    /// </summary>
    /// <param name="command">Command parameters.</param>
    Task UpdateAsync(UpdatePageAccessRuleSetCommand command);

    /// <summary>
    /// Updates all access rules associated with a page.
    /// </summary>
    /// <param name="pageId">
    /// Database id of the page to update.
    /// </param>
    /// <param name="commandPatcher">
    /// An action to configure or "patch" a command that's been initialized
    /// with the existing page access rule data.
    /// </param>
    Task UpdateAsync(int pageId, Action<UpdatePageAccessRuleSetCommand> commandPatcher);
}
