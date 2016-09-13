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
    /// Use this to decorate a DateTime field and provide a UI hint to the admin interface to display a date picker field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DateAttribute : Attribute, IMetadataAttribute
    {
        public void Process(ModelMetadata modelMetaData)
        {
            modelMetaData.TemplateHint = "Date";
        }
    }
}
