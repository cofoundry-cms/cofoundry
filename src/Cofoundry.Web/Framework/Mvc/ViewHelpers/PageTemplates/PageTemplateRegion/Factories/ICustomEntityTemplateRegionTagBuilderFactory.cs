using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Factory that enables ICustomEntityTemplateRegionTagBuilder implementation to be swapped out.
    /// </summary>
    /// <remarks>
    /// The factory is required because the HtmlHelper cannot be injected
    /// </remarks>
    public interface ICustomEntityTemplateRegionTagBuilderFactory
    {
        ICustomEntityTemplateRegionTagBuilder<TModel> Create<TModel>(
            ViewContext viewContext,
            ICustomEntityPageViewModel<TModel> customEntityViewModel,
            string regionName
            )
            where TModel : ICustomEntityPageDisplayModel
            ;
    }
}
