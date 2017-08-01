using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Don't implement this directly, use the generic version.
    /// </summary>
    public interface ICustomEntityPageDisplayModel : ICustomEntityDisplayModel
    {
        string PageTitle { get; set; }

        string MetaDescription { get; set; }
    }

    /// <summary>
    /// Use this to register a display view model for a custom entity page. This is similar to
    /// ICustomEntityDisplayModel, except it includes extra page meta data that must be mapped.
    /// You must use this generic version so that we can link the model back to a ICustomEntityDataModel.
    /// </summary>
    /// <typeparam name="TDataModel">ICustomEntityDataModel type</typeparam>
    public interface ICustomEntityPageDisplayModel<TDataModel> : ICustomEntityPageDisplayModel, ICustomEntityDisplayModel<TDataModel>
        where TDataModel : ICustomEntityDataModel
    {
    }
}