﻿using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cofoundry.Domain.Internal;

public class DynamicDataModelSchemaMapper : IDynamicDataModelSchemaMapper
{
    private readonly IModelMetadataProvider _modelMetadataProvider;

    public DynamicDataModelSchemaMapper(
        IModelMetadataProvider modelMetadataProvider
        )
    {
        _modelMetadataProvider = modelMetadataProvider;
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
            property.Name = propertyMetaData.PropertyName;
            property.DisplayName = propertyMetaData.DisplayName;
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

        // A default constructor is required for data model seralization anyway
        // so that constraint isn't an issue here.
        details.DefaultValue = new DynamicDataModelDefaultValue();
        details.DefaultValue.Value = Activator.CreateInstance(modelType);
    }
}
