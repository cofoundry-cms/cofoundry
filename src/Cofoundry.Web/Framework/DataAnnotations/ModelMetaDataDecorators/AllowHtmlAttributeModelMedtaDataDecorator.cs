using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class AllowHtmlAttributeModelMedtaDataDecorator : IModelMetaDataDecorator
    {
        public bool CanDecorateType(Type type)
        {
            return type == typeof(AllowHtmlAttribute);
        }

        public void Decorate(Attribute attribute, System.Web.Mvc.ModelMetadata modelMetaData)
        {
            modelMetaData.TemplateHint = "Html";
        }
    }
}
