using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageTemplateCustomEntityTypeMapper"/>.
/// </summary>
public class PageTemplateCustomEntityTypeMapper : IPageTemplateCustomEntityTypeMapper
{
    private readonly IEnumerable<ICustomEntityDisplayModel> _customEntityDisplayModels;
    private readonly ILogger<PageTemplateCustomEntityTypeMapper> _logger;

    public PageTemplateCustomEntityTypeMapper(
        IEnumerable<ICustomEntityDisplayModel> customEntityDisplayModels,
        ILogger<PageTemplateCustomEntityTypeMapper> logger
        )
    {
        _customEntityDisplayModels = customEntityDisplayModels;
        _logger = logger;
    }

    /// <inheritdoc/>
    public virtual Type? Map(string? typeName)
    {
        typeName = RemoveNamespace(typeName);
        if (string.IsNullOrEmpty(typeName))
        {
            return null;
        }

        var displayModels = _customEntityDisplayModels.Where(m => m.GetType().Name == typeName);

        if (displayModels.Count() > 1)
        {
            _logger.LogWarning("Incorrect number of ICustomEntityDisplayModels registered with the name '{TypeName}'. Expected 1, got {NumModels}", typeName, displayModels.Count());
        }

        Type? result = null;

        if (displayModels.Any())
        {
            result = displayModels.First().GetType();
        }

        return result;
    }

    protected static string? RemoveNamespace(string? typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return null;
        }

        var dotIndex = typeName.LastIndexOf('.');
        if (dotIndex != -1 && dotIndex < typeName.Length)
        {
            typeName = typeName.Substring(dotIndex + 1);
        }

        return typeName;
    }
}
