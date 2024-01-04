using Cofoundry.Domain.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cofoundry.Domain.Internal;

public class DynamicDataModelSchemaMapper : IDynamicDataModelSchemaMapper
{
    private readonly IModelMetadataProvider _modelMetadataProvider;
    private readonly IEmptyDataModelFactory _emptyDataModelFactory;

    public DynamicDataModelSchemaMapper(
        IModelMetadataProvider modelMetadataProvider,
        IEmptyDataModelFactory emptyDataModelFactory
        )
    {
        _modelMetadataProvider = modelMetadataProvider;
        _emptyDataModelFactory = emptyDataModelFactory;
    }

    public void Map(IDynamicDataModelSchema details, Type modelType)
    {
        ArgumentNullException.ThrowIfNull(details);
        ArgumentNullException.ThrowIfNull(modelType);

        var dataModelMetaData = _modelMetadataProvider.GetMetadataForType(modelType);

        details.DataTemplateName = StringHelper.FirstNotNullOrWhitespace(
            dataModelMetaData.TemplateHint,
            dataModelMetaData.DataTypeName
            ) ?? string.Empty;

        var properiesMetaData = _modelMetadataProvider.GetMetadataForProperties(modelType);

        var dataModelProperties = new List<DynamicDataModelSchemaProperty>();
        foreach (var propertyMetaData in properiesMetaData.OrderBy(p => p.Order))
        {
            var property = new DynamicDataModelSchemaProperty();
            if (propertyMetaData.PropertyName == null)
            {
                throw new InvalidOperationException($"{nameof(propertyMetaData.PropertyName)} is not expected to be null for property metadata");
            }
            property.Name = propertyMetaData.PropertyName;
            property.DisplayName = propertyMetaData.DisplayName ?? propertyMetaData.PropertyName;
            property.Description = propertyMetaData.Description;
            property.IsRequired = propertyMetaData.IsRequired;

            property.DataTemplateName = StringHelper.FirstNotNullOrWhitespace(
                propertyMetaData.TemplateHint,
                propertyMetaData.DataTypeName,
                propertyMetaData.IsNullableValueType ? propertyMetaData.ModelType.GenericTypeArguments[0].Name : propertyMetaData.ModelType.Name
                ) ?? string.Empty;

            // Not sure why the keys here could be objects, but we're not interested in 
            // them if they are.
            property.AdditionalAttributes = propertyMetaData
                .AdditionalValues
                .Where(d => d.Key is string)
                .ToDictionary(d => (string)d.Key, d => d.Value);

            dataModelProperties.Add(property);
        }

        details.DataModelProperties = dataModelProperties.ToArray();
        details.DefaultValue = new()
        {
            Value = _emptyDataModelFactory.Create<object>(modelType)
        };
    }
}
