using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Web;

namespace Cofoundry.Web
{
    /// <summary>
    /// Fluent builder for defining a page template section configuration using method chaining
    /// </summary>
    public interface IPageTemplateSectionTagBuilder : IHtmlString
    {
        /// <summary>
        /// Permits the page module associated with the specified data model type
        /// to be selected and used in this page section. By default all modules are 
        /// available but using this method changes the module selection to an 'opt-in'
        /// configuration.
        /// </summary>
        /// <typeparam name="TModuleType">The data model type of the page module to register</typeparam>
        /// <returns>IPageTemplateSectionTagBuilder for method chaining</returns>
        IPageTemplateSectionTagBuilder AllowModuleType<TModuleType>() where TModuleType : IPageModuleDataModel;

        /// <summary>
        /// Permits the specified page module to be selected and used in this page 
        /// section. By default all modules are available but using this method changes 
        /// the module selection to an 'opt-in' configuration.
        /// </summary>
        /// <param name="moduleType">The name of the moduletype to register e.g. "RawHtml" or "PlainText"</param>
        /// <returns>IPageTemplateSectionTagBuilder for method chaining</returns>
        IPageTemplateSectionTagBuilder AllowModuleType(string moduleType);

        /// <summary>
        /// Permits the specified page modules to be selected and used in this page 
        /// section. By default all modules are available but using this method changes 
        /// the module selection to an 'opt-in' configuration.
        /// configuration.
        /// </summary>
        /// <param name="moduleTypes">The names of the moduletype to register e.g. "RawHtml" or "PlainText"</param>
        /// <returns>IPageTemplateSectionTagBuilder for method chaining</returns>
        IPageTemplateSectionTagBuilder AllowModuleTypes(params string[] moduleTypes);

        /// <summary>
        /// By default only a single module is allowed per section. This method configures the
        /// section to allow an unlimited number of modules to be added
        /// </summary>
        /// <returns>IPageTemplateSectionTagBuilder for method chaining</returns>
        IPageTemplateSectionTagBuilder AllowMultipleModules();

        /// <summary>
        /// Sets the default height of the section when there it contains no
        /// modules and the page is in edit mode. This makes it easier to visualize
        /// and select the module when a template is first created.
        /// </summary>
        /// <param name="minHeight">The minimum height in pixels</param>
        /// <returns>IPageTemplateSectionTagBuilder for method chaining</returns>
        IPageTemplateSectionTagBuilder EmptyContentMinHeight(int minHeight);

        /// <summary>
        /// Wraps page modules within the section in a specific html tag. Each section
        /// needs to have a single container node in page edit mode and by default a div
        /// element is used if a single container cannot be found in the rendered output.
        /// Use this setting to control this tag. If a tag is specified here the output
        /// will always be wrapped irrespecitive of whether the page is in edit mode to maintain
        /// consistency.
        /// </summary>
        /// <param name="tagName">Name of the element to wrap the output in e.g. div, p, header</param>
        /// <param name="htmlAttributes">Html attributes to apply to the wrapping tag.</param>
        /// <returns>IPageTemplateSectionTagBuilder for method chaining</returns>
        IPageTemplateSectionTagBuilder WrapWithTag(string tagName, object htmlAttributes = null);
    }
}
