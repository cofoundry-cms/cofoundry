namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageBlockTypeDataModelTypeFactory"/>.
/// </summary>
public class PageBlockTypeDataModelTypeFactory : IPageBlockTypeDataModelTypeFactory
{
    private readonly IEnumerable<IPageBlockTypeDataModel> _allPageBlockTypeDataModels;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IPageBlockTypeFileNameFormatter _pageBlockTypeFileNameFormatter;

    public PageBlockTypeDataModelTypeFactory(
        IEnumerable<IPageBlockTypeDataModel> allPageBlockTypeDataModels,
        IQueryExecutor queryExecutor,
        IPageBlockTypeFileNameFormatter pageBlockTypeFileNameFormatter
        )
    {
        _allPageBlockTypeDataModels = allPageBlockTypeDataModels;
        _queryExecutor = queryExecutor;
        _pageBlockTypeFileNameFormatter = pageBlockTypeFileNameFormatter;
    }

    /// <inheritdoc/>
    public Type CreateByPageBlockTypeFileName(string pageBlockTypeFileName)
    {
        // take advantage of the cached type list in the dependency resolver to get a collection of DataModel instances
        // we dont actually use these instances, we just use them to get the type. A bit wasteful perhaps but object creation is cheap 
        // and the only alternative is searching through all assembly types which is very slow.
        var dataModelTypes = _allPageBlockTypeDataModels
            .Where(m => !(m is UninitializedPageBlockTypeDataModel))
            .Select(t => t.GetType())
            .Where(t => _pageBlockTypeFileNameFormatter.FormatFromDataModelType(t).Equals(pageBlockTypeFileName, StringComparison.OrdinalIgnoreCase));

        if (!dataModelTypes.Any())
        {
            throw new InvalidOperationException($"DataModel for page block type {pageBlockTypeFileName} not yet implemented");
        }

        if (dataModelTypes.Count() > 1)
        {
            throw new InvalidOperationException($"DataModel for page block type {pageBlockTypeFileName} is registered multiple times. Please use unique class names.");
        }

        return dataModelTypes.First();
    }

    /// <inheritdoc/>
    public async Task<Type> CreateByPageBlockTypeIdAsync(int pageBlockTypeId)
    {
        var query = new GetPageBlockTypeSummaryByIdQuery(pageBlockTypeId);
        var blockType = await _queryExecutor.ExecuteAsync(query);
        EntityNotFoundException.ThrowIfNull(blockType, pageBlockTypeId);

        return CreateByPageBlockTypeFileName(blockType.FileName);
    }
}
