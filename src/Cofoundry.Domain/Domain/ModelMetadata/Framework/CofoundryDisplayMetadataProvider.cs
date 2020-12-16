using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class CofoundryDisplayMetadataProvider : IDisplayMetadataProvider
    {
        #region constructor

        private readonly IEnumerable<IModelMetadataDecorator> _modelMetaDataDecorators;

        public CofoundryDisplayMetadataProvider(
            IEnumerable<IModelMetadataDecorator> modelMetaDataDecorators
            )
        {
            _modelMetaDataDecorators = modelMetaDataDecorators;
        }

        #endregion

        public virtual void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            var modelMetadata = context.DisplayMetadata;

            if (modelMetadata.DisplayName == null && !string.IsNullOrEmpty(context.Key.Name))
            {
                modelMetadata.DisplayName = () => HumanizePropertyName(context.Key.Name);
            }

            foreach (var attribute in context.Attributes)
            {
                if (attribute is IMetadataAttribute)
                {
                    ((IMetadataAttribute)attribute).Process(context);
                }
                else
                {
                    var decorator = _modelMetaDataDecorators.FirstOrDefault(d => d.CanDecorateType(attribute.GetType()));

                    if (decorator != null)
                    {
                        decorator.Decorate(attribute, context);
                    }
                }
            }
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
