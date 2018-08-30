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
            MetaDataAttributePlacementValidator.ValidatePropertyType(this, context, typeof(int), typeof(int?));

            context
                .DisplayMetadata
                .AdditionalValues
                .Add("PreviewImage", true)
                ;
        }
    }
}
