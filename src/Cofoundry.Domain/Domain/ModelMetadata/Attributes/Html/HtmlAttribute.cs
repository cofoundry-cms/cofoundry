using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

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

        public void Process(DisplayMetadata modelMetaData)
        {
            modelMetaData
                .AddAdditionalValueIfNotEmpty("Toolbars", Toolbars)
                .AddAdditionalValueIfNotEmpty("CustomToolbar", CustomToolbar)
                .TemplateHint = DataType.Html.ToString();
        }

        /// <summary>
        /// The toolbars to display. A default selection is used if this is not specified.
        /// </summary>
        public ICollection<HtmlToolbarPreset> Toolbars { get; private set; }

        /// <summary>
        /// https://www.tinymce.com/docs/advanced/editor-control-identifiers/#toolbarcontrols
        /// </summary>
        public string CustomToolbar { get; set; }
    }
}
