namespace Cofoundry.Domain.MailTemplates.Internal
{
    /// <inheritdoc/>
    public class CofoundryMailTemplateHelper : ICofoundryMailTemplateHelper
    {
        public CofoundryMailTemplateHelper(
            IContentRouteLibrary contentRouteLibrary,
            IHtmlSanitizerHelper htmlSanitizerHelper
            )
        {
            Routing = contentRouteLibrary;
            Sanitizer = htmlSanitizerHelper;
        }

        public IContentRouteLibrary Routing { get; private set; }

        public IHtmlSanitizerHelper Sanitizer { get; private set; }
    }
}