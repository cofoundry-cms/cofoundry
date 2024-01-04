namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageBlockTypeDetailsMapper"/>.
/// </summary>
public class PageBlockTypeDetailsMapper : IPageBlockTypeDetailsMapper
{
    private readonly IDynamicDataModelSchemaMapper _dynamicDataModelTypeMapper;
    private readonly IEnumerable<IPageBlockTypeDataModel> _allPageBlockTypeDataModels;

    public PageBlockTypeDetailsMapper(
        IDynamicDataModelSchemaMapper dynamicDataModelTypeMapper,
        IEnumerable<IPageBlockTypeDataModel> allPageBlockTypeDataModels
        )
    {
        _dynamicDataModelTypeMapper = dynamicDataModelTypeMapper;
        _allPageBlockTypeDataModels = allPageBlockTypeDataModels;
    }

    /// <inheritdoc/>
    public PageBlockTypeDetails Map(PageBlockTypeSummary blockTypeSummary)
    {
        var result = new PageBlockTypeDetails()
        {
            Description = blockTypeSummary.Description,
            FileName = blockTypeSummary.FileName,
            Name = blockTypeSummary.Name,
            PageBlockTypeId = blockTypeSummary.PageBlockTypeId,
            Templates = blockTypeSummary.Templates
        };

        var dataModelType = GetPageBlockDataModelType(result);

        _dynamicDataModelTypeMapper.Map(result, dataModelType);

        return result;
    }

    private Type GetPageBlockDataModelType(PageBlockTypeDetails blockTypeDetails)
    {
        var dataModelName = blockTypeDetails.FileName + "DataModel";

        var dataModel = _allPageBlockTypeDataModels
            .Select(m => m.GetType())
            .Where(m => m.Name == dataModelName)
            .SingleOrDefault();

        EntityNotFoundException.ThrowIfNull(dataModel, dataModelName);

        return dataModel;
    }
}
