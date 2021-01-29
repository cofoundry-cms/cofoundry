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
    /// Use this to decorate a string property and provide a UI hint to the admin interface 
    /// to display a color picker field and validate a hexadecimal color value
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColorAttribute : RegularExpressionAttribute, IMetadataAttribute
    {
        const string REGEX = "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";
        public ColorAttribute() :
            base(REGEX)
        {
            ErrorMessage = "{0} must be a hexadecimal colour value e.g. '#EFEFEF' or '#fff'";
        }

        public void Process(DisplayMetadataProviderContext context)
        {
            var modelMetaData = context.DisplayMetadata;
            modelMetaData.TemplateHint = "Color";
        }
    }
}
