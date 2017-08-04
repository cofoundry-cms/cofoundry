using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This attribute can be applied to string properties on a dynamic data provider to show that they can be searched for text for use with site search
    /// </summary>
    /// <remarks>
    /// Copied from Data project but not really implemented yet, still need to be worked on.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SearchableAttribute : Attribute
    {
    }
}
