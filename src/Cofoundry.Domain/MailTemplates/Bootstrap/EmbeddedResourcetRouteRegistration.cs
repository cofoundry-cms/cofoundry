using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.EmbeddedResources;

namespace Cofoundry.Domain.MailTemplates
{
    public class EmbeddedResourcetRouteRegistration : IEmbeddedResourceRouteRegistration
    {
        public IEnumerable<string> GetEmbeddedResourcePaths()
        {
            yield return TemplatePath.Root + "shared/content";
        }
    }
}