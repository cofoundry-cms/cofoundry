using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Indicates the property of a model that can be used as a
    /// title, name or short textual identifier. Typically this is
    /// used in a grid of items to identify the row.
    /// </summary>
    public class PreviewTitleAttribute : Attribute, IMetadataAttribute
    {
        public void Process(DisplayMetadataProviderContext context)
        {
            context
                .DisplayMetadata
                .AdditionalValues
                .Add("PreviewTitle", true)
                ;
        }
    }
}
