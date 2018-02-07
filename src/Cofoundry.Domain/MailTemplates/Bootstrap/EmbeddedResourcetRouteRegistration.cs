using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Domain.MailTemplates
{
    public class EmbeddedResourcetRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        public IEnumerable<EmbeddedResourcePath> GetEmbeddedResourcePaths()
        {
            var path = new EmbeddedResourcePath(
                GetType().Assembly,
                TemplatePath.Root + "shared/content"
                );

            yield return path;
        }
    }
}