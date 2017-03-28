using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Used to read view files from which are typically stored
    /// either on disk or in an embedded resource.
    /// </summary>
    public interface IViewFileReader
    {
        string Read(string viewPath);
    }
}
