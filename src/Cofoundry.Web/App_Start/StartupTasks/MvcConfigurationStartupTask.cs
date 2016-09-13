using Cofoundry.Domain;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Misc setup configuration of the MVC framework. Disables the version header, registers 
    /// some modelbinders and adds Cofoundry to the default controller namespaces.
    /// </summary>
    public class MvcConfigurationStartupTask : IStartupTask
    {
        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Normal; }
        }

        public void Run(IAppBuilder app)
        {
            // Remove the HTTP header that says X-AspNetMvc-Version
            MvcHandler.DisableMvcResponseHeader = true;

            ControllerBuilder.Current.DefaultNamespaces.Add(typeof(PagesController).Namespace);

            RegisterModelBinders();
        }

        #region helpers

        private static void RegisterModelBinders()
        {
            ModelBinders.Binders.Add(typeof(ImageAnchorLocation), new EnumBinder<ImageAnchorLocation>(null));
        }

        #endregion
    }
}