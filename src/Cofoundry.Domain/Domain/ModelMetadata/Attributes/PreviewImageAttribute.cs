using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Indicates the property of a model that can be used as the
    /// main image when displaying the model. Typically this is
    /// used in a grid of items to show an image representation of the row.
    /// </summary>
    public class PreviewImageAttribute : Attribute, IMetadataAttribute
    {
        public void Process(DisplayMetadataProviderContext context)
        {
            if (context.Key.ModelType != typeof(int) && context.Key.ModelType != typeof(int?))
            {
                var msg = $"{typeof(PreviewImageAttribute).Name} can only be placed on properties with an int or int? type.";
                throw new Exception(msg);
            }

            context
                .DisplayMetadata
                .AdditionalValues
                .Add("PreviewImage", true)
                ;
        }
    }
}
