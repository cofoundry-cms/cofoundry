using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cofoundry.Core.EmbeddedResources;

namespace Cofoundry.Core.Mail
{
    public class ViewFileReader : IViewFileReader
    {
        private readonly IResourceLocator _resourceLocator;

        public ViewFileReader(
            IResourceLocator assemblyResourceLocator
            )
        {
            _resourceLocator = assemblyResourceLocator;
        }

        public string Read(string viewPath)
        {
            if (!_resourceLocator.FileExists(viewPath))
            {
                throw new InvalidOperationException(string.Format("Could not find template {0}", viewPath));
            }

            var file = _resourceLocator.GetFile(viewPath);
            string template = null;

            using (var stream = file.Open())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                template = reader.ReadToEnd();
            }

            return template;
        }
    }
}
