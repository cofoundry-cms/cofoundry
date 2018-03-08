using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this to add additional meta information to a ModelMetadata object for attributes
    /// that you have no control over (i.e. DataAnnotation framework attributes)
    /// </summary>
    public interface IModelMetadataDecorator
    {
        bool CanDecorateType(Type type);
        void Decorate(object attribute, DisplayMetadataProviderContext context);
    }
}
