using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Use this to add additional meta information to a ModelMetadata object for attributes
    /// that you have no control over (i.e. DataAnnotation framework attributes)
    /// </summary>
    public interface IModelMetaDataDecorator
    {
        bool CanDecorateType(Type type);
        void Decorate(Attribute attribute, ModelMetadata modelMetaData);
    }
}
