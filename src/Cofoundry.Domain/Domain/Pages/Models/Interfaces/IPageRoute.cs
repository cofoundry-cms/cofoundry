using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a page model that includes data for buiding a url.
    /// </summary>
    public interface IPageRoute
    {
        /// <summary>
        /// The full path of the page including directories and the locale. 
        /// </summary>
        string FullPath { get; }
    }
}
