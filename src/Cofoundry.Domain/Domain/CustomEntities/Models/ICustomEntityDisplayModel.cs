using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Don't inherit from this directly, use the generic version
    /// </summary>
    public interface ICustomEntityDisplayModel
    {
    }

    /// <summary>
    /// Use this to register a display view model for a custom entity. Display models
    /// are used to render out views and are enhanced versions of the data model stored
    /// in the database. This is useful for adding additional data or pulling in related 
    /// entities since the data model typically only stores keys to related data.
    /// </summary>
    /// <remarks>
    /// You must use this generic version so that we can link the model back to an ICustomEntityDataModel.
    /// </remarks>
    /// <typeparam name="TDataModel">ICustomEntityDataModel type</typeparam>
    public interface ICustomEntityDisplayModel<TDataModel> : ICustomEntityDisplayModel 
        where TDataModel : ICustomEntityDataModel
    {
    }
}
