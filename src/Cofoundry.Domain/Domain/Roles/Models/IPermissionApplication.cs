using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represemts a permission applied to an object. Typically this can just be
    /// an IPermission object, but permissions could be applied in otherways, e.g.
    /// in an OR relationship using CompositePermissionApplication
    /// </summary>
    public interface IPermissionApplication
    {
    }
}
