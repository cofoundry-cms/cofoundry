using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Don't implement this directly, use the generic version
    /// </summary>
    public interface ICustomEntityDetailsDisplayViewModel : ICustomEntityDisplayModel
    {
        string PageTitle { get; set; }

        string MetaDescription { get; set; }
    }

    /// <summary>
    /// Use this to register a display view model for a custom entity details page. You must use this generic
    /// version so that we can link the model back to a ICustomEntityVersionDataModel
    /// </summary>
    /// <typeparam name="TDataModel">ICustomEntityVersionDataModel type</typeparam>
    public interface ICustomEntityDetailsDisplayViewModel<TDataModel> : ICustomEntityDetailsDisplayViewModel, ICustomEntityDisplayModel<TDataModel>
        where TDataModel : ICustomEntityVersionDataModel
    {
    }
}