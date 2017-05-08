using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Core.Mail
{
    public class ViewFileReader : IViewFileReader
    {
        private readonly IResourceLocator _resourceLocator;

        public ViewFileReader(
            IResourceLocator resourceLocator
            )
        {
            _resourceLocator = resourceLocator;
        }

        public string Read(string viewPath)
        {
            var file = _resourceLocator.GetFile(viewPath);

            if (!file.Exists || file.IsDirectory)
            {
                throw new InvalidOperationException(string.Format("Could not find template {0}", viewPath));
            }

            string template = null;

            using (var stream = file.CreateReadStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                template = reader.ReadToEnd();
            }

            return template;
        }
    }
}
