using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Indicates the property of a model that can be used as a
    /// description field. Typically this is used in a grid of items 
    /// to describe the item.
    /// </summary>
    public class PreviewDescriptionAttribute : Attribute, IMetadataAttribute
    {
        public void Process(DisplayMetadataProviderContext context)
        {
            context
                .DisplayMetadata
                .AdditionalValues
                .Add("PreviewDescription", true)
                ;
        }
    }
}
