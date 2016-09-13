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
    /// Use this to register a display view model for a custom entity. You must use this generic
    /// version so that we can link the model back to a ICustomEntityVersionDataModel
    /// </summary>
    /// <typeparam name="TDataModel">ICustomEntityVersionDataModel type</typeparam>
    public interface ICustomEntityDisplayModel<TDataModel> : ICustomEntityDisplayModel 
        where TDataModel : ICustomEntityVersionDataModel
    {
    }
}
