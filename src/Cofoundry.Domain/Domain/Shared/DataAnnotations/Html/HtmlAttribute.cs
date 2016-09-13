using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this to decorate a string field and provide a UI hint to the admin 
    /// interface to display an html editor field. Toolbar options can be 
    /// specified in the constructor and the CustomToolbar property can be 
    /// used to show a completely custom toolbar.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class HtmlAttribute : Attribute, IMetadataAttribute
    {
        /// <summary>
        /// Optionally provide details of the toolbars to be displayed.
        /// </summary>
        /// <param name="toolbars">The toolbars to display. A default selection is used if this is not specified.</param>
        public HtmlAttribute(params HtmlToolbarPreset[] toolbars)
        {
            Toolbars = toolbars;
        }

        public void Process(ModelMetadata modelMetaData)
        {
            modelMetaData
                .AddAdditionalValueIfNotEmpty("Toolbars", Toolbars)
                .AddAdditionalValueIfNotEmpty("CustomToolbar", CustomToolbar)
                .TemplateHint = DataType.Html.ToString();
        }

        /// <summary>
        /// The toolbars to display. A default selection is used if this is not specified.
        /// </summary>
        public HtmlToolbarPreset[] Toolbars { get; private set; }

        /// <summary>
        /// Custom toolbar string to pass through to the client. Should be in the 
        /// format "['h1','h2','h3'],['bold','italics']". For syntax/options
        /// see https://github.com/fraywing/textAngular/wiki/Customising-The-Toolbar
        /// but don't include the outer array brackets.
        /// </summary>
        public string CustomToolbar { get; set; }
    }
}
