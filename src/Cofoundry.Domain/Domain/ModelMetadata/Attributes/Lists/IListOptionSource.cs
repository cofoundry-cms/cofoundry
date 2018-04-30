using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this interface to define a static data source for a list
    /// component in the admin UI.
    /// </summary>
    public interface IListOptionSource
    {
        /// <summary>
        /// Creates the list of options.
        /// </summary>
        ICollection<ListOption> Create();
    }
}
