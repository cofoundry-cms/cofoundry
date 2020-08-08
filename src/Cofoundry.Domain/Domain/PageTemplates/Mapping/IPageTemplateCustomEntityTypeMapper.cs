using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to look up custom entity display model types from a string type 
    /// name. Used specifically when extracting the custom entity type from a
    /// razor view template.
    /// </summary>
    public interface IPageTemplateCustomEntityTypeMapper
    {
        /// <summary>
        /// Takes string type name and attempts to map it to a type that
        /// implements ICustomEntityDisplayModel. If one is found it is returned
        /// otherwise null is returned.
        /// </summary>
        /// <param name="typeName">
        /// Type name to look for. This is case sensitive and the namespace can 
        /// be included (but isn't checked).
        /// </param>
        /// <returns>ICustomEntityDisplayModel type if a match is found; otherwise null.</returns>
        Type Map(string typeName);
    }
}
