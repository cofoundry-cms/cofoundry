using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This can be used to decorate a VimeoVideo property and provide a UI Hint
    /// to the admin interface to display a Vimeo video picker.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class VimeoAttribute : Attribute, IMetadataAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VimeoAttribute"/> class.
        /// </summary>
        public VimeoAttribute()
            : base()
        {
            
        }

        public void Process(DisplayMetadataProviderContext context)
        {
            context.DisplayMetadata.TemplateHint = "Vimeo";
        }
    }
}