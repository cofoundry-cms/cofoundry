using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Shared helper between add/update page block commands for updating the db model
/// </summary>
public interface IPageBlockCommandHelper
{
    Task UpdateModelAsync(IPageVersionBlockDataModelCommand command, IEntityVersionPageBlock dbModule);
}
