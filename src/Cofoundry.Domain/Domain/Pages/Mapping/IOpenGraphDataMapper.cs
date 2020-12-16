using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to OpenGraphData objects.
    /// </summary>
    public interface IOpenGraphDataMapper
    {
        /// <summary>
        /// Maps an EF PageVersion record from the db into an OpenGraphData 
        /// object.
        /// </summary>
        /// <param name="dbPageVersion">PageVersion record from the database. Must include the OpenGraphImageAsset property.</param>
        OpenGraphData Map(PageVersion dbPageVersion);
    }
}
