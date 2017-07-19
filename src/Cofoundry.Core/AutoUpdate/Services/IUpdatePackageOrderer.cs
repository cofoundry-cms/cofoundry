using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Order UpdatePackage instances into the correct execution order, making
    /// sure dependencies are taken into consideration.
    /// </summary>
    public interface IUpdatePackageOrderer
    {
        /// <summary>
        /// Orders a collection of UpdatePackage instances into the correct execution 
        /// order, making sure dependencies are taken into consideration.
        /// </summary>
        /// <param name="packages">update packages to sort.</param>
        /// <returns>Enumerable collection of update packages, sorted into the correct order.</returns>
        IEnumerable<UpdatePackage> Order(IList<UpdatePackage> packages);
    }
}
