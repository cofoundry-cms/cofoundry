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
    /// Use this to decorate a string field and provide a UI hint to the admin interface to display a text area field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MultiLineTextAttribute : Attribute, IMetadataAttribute
    {
        public void Process(ModelMetadata modelMetaData)
        {
            modelMetaData.TemplateHint = DataType.MultilineText.ToString();
        }
    }
}
