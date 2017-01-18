using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this to decorate a string field and provide a UI hint to the admin interface 
    /// to display a color picker field and validates a hexidecimal color value
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

        public void Process(ModelMetadata modelMetaData)
        {
            modelMetaData.TemplateHint = "Color";
        }
    }
}
