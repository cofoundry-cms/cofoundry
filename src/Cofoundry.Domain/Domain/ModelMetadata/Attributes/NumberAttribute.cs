using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this to decorate a numeric field and provide a UI hint to the admin interface 
    /// to display an html5 number field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NumberAttribute : Attribute, IMetadataAttribute
    {
        /// <summary>
        /// Maps to the step attribute on a number html input field. This can be used to 
        /// control the precision of the number entered. E.g. use a step value of '0.1' to 
        /// allow a decimal value to 1 decimal place. The default value is '1' which 
        /// uses integer level precision. A special value of 'any' can be used to allow
        /// any number type.
        /// </summary>
        public string Step { get; set; }

        public void Process(DisplayMetadataProviderContext context)
        {

            context.DisplayMetadata.TemplateHint = "Number";

            context.DisplayMetadata
                .AddAdditionalValueIfNotEmpty("Step", Step)
                ;
        }
    }
}
