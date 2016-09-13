using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class DynamicDataModelSchemaMapper
    {
        private readonly ModelMetadataProvider _modelMetadataProvider;

        public DynamicDataModelSchemaMapper(
            ModelMetadataProvider modelMetadataProvider
            )
        {
            _modelMetadataProvider = modelMetadataProvider;
        }

        public void Map(IDynamicDataModelSchema details, Type modelType)
        {
            Condition.Requires(details).IsNotNull();
            Condition.Requires(modelType).IsNotNull();

            var dataModelMetaData = _modelMetadataProvider.GetMetadataForType(null, modelType);

            details.DataTemplateName = StringHelper.FirstNonEmpty(
                dataModelMetaData.TemplateHint,
                dataModelMetaData.DataTypeName
                );

            var properiesMetaData = _modelMetadataProvider.GetMetadataForProperties(null, modelType);

            var dataModelProperties = new List<DynamicDataModelSchemaProperty>();
            foreach (var propertyMetaData in properiesMetaData.OrderBy(p => p.Order))
            {
                var property = new DynamicDataModelSchemaProperty();
                property.Name = propertyMetaData.PropertyName;
                property.DisplayName = propertyMetaData.DisplayName;
                property.Description = propertyMetaData.Description;
                property.IsRequired = propertyMetaData.IsRequired;

                property.DataTemplateName = StringHelper.FirstNonEmpty(
                    propertyMetaData.TemplateHint,
                    propertyMetaData.DataTypeName,
                    propertyMetaData.IsNullableValueType ? propertyMetaData.ModelType.GenericTypeArguments[0].Name : propertyMetaData.ModelType.Name
                    );

                property.AdditionalAttributes = propertyMetaData.AdditionalValues;

                dataModelProperties.Add(property);
            }

            details.DataModelProperties = dataModelProperties.ToArray();
        }
    }
}
