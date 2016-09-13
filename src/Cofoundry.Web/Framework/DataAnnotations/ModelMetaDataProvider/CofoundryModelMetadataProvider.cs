using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cofoundry.Domain;
using Cofoundry.Core;

namespace Cofoundry.Web
{
    /// <summary>
    /// A custom MetaData provider to enable extra attributes. Additonal attributes
    /// should implement IMetadataAttribute.
    /// </summary>
    public class CofoundryModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        #region constructor

        private readonly IModelMetaDataDecorator[] _modelMetaDataDecorators;

        public CofoundryModelMetadataProvider(
            IModelMetaDataDecorator[] modelMetaDataDecorators
            )
        {
            _modelMetaDataDecorators = modelMetaDataDecorators;
        }

        #endregion

        protected override ModelMetadata CreateMetadata(
            IEnumerable<Attribute> attributes,
            Type containerType,
            Func<object> modelAccessor,
            Type modelType,
            string propertyName)
        {
            var modelMetadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            if (modelMetadata.DisplayName == null && !string.IsNullOrEmpty(propertyName))
            {
                modelMetadata.DisplayName = HumanizePropertyName(propertyName);
            }

            foreach (var attribute in attributes)
            {
                if (attribute is IMetadataAttribute)
                {
                    ((IMetadataAttribute)attribute).Process(modelMetadata);
                }
                else
                {
                    var decorator = _modelMetaDataDecorators.FirstOrDefault(d => d.CanDecorateType(attribute.GetType()));

                    if (decorator != null)
                    {
                        decorator.Decorate(attribute, modelMetadata);
                    }
                }
            }

            return modelMetadata;
        }

        /// <summary>
        /// Humanizes a property name by removing an Id suffix if one exists and splitting
        /// on pascal case word boundaries.
        /// </summary>
        public static string HumanizePropertyName(string propertyName)
        {
            const string ID_SUFFIX = "Id";

            if (propertyName.EndsWith(ID_SUFFIX))
            {
                propertyName = propertyName.Substring(0, propertyName.Length - ID_SUFFIX.Length);
            }

            return TextFormatter.PascalCaseToSentence(propertyName);
        }
    }
}
