using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This can be used to decorate a YouTubeVideo property and provide a UI Hint
    /// to the admin interface to display a YouTube video picker.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class YouTubeAttribute : Attribute, IMetadataAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YouTubeAttribute"/> class.
        /// </summary>
        public YouTubeAttribute()
            : base()
        {
        }

        public void Process(DisplayMetadataProviderContext context)
        {
            // NB: must use lowercase t because the directive needs to transform 
            // to form-field-youtube with youtube as one word.
            context.DisplayMetadata.TemplateHint = "Youtube";
        }
    }
}