using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.ResourceFiles
{
    public interface IResourceFileProviderRegistration
    {
        IEnumerable<IFileProvider> GetFileProviders();
    }
}
