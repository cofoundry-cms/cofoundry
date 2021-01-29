using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this to decorate a string property and provide a UI hint to 
    /// the admin interface to display a text area field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MultiLineTextAttribute : Attribute, IMetadataAttribute
    {
        public MultiLineTextAttribute()
        {
            Rows = 4;
        }

        /// <summary>
        /// The number of visible lines of text in the text editor.
        /// Defaults to 4.
        /// </summary>
        public int Rows { get; set; }

        public void Process(DisplayMetadataProviderContext context)
        {
            var modelMetaData = context
                .DisplayMetadata
                .AddAdditionalValueIfNotEmpty("Rows", Rows)
                .TemplateHint = DataType.MultilineText.ToString();

        }
    }
}
