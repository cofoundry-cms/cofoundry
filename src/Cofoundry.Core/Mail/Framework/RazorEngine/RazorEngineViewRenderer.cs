using RazorEngine.Configuration;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    public class RazorEngineViewRenderer : IMailViewRenderer
    {
        private readonly IRazorEngineService _razorEngine;
        private readonly ITemplateManager _templateManager;
        private readonly IViewFileReader _viewFileReader;

        public RazorEngineViewRenderer(
            ITemplateManager templateManager,
            IViewFileReader viewFileReader,
            IActivator activator
            )
        {
            _viewFileReader = viewFileReader;
            var config = new FluentTemplateServiceConfiguration(x => x
                .ManageUsing(templateManager)
                .ActivateUsing(activator)
                //.IncludeNamespaces() - could add some default namespaces here if need be
                );

            _razorEngine = RazorEngineService.Create(config);
            _templateManager = templateManager;
        }

        public string Render(string viewPath)
        {
            var template = _viewFileReader.Read(viewPath);
            var view = _razorEngine.RunCompile(template, viewPath);

            return view;
        }

        public string Render<T>(string viewPath, T model)
        {
            var template = _viewFileReader.Read(viewPath);
            var view = _razorEngine.RunCompile(viewPath, model.GetType(), model);

            return view;
        }
    }
}
