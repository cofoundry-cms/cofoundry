using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Configuration
{
    /// <summary>
    /// Marks IConfigurationSettings that shoudl; have a higher level namespace
    /// </summary>
    public interface INamespacedConfigurationSettings : IConfigurationSettings
    {
        string Namespace { get; }
    }
}
