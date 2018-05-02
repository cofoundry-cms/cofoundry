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
            Rows = 20;
        }

        public void Process(DisplayMetadataProviderContext context)
        {
            context.DisplayMetadata
                .AddAdditionalValueIfNotEmpty("Toolbars", Toolbars)
                .AddAdditionalValueIfNotEmpty("CustomToolbar", CustomToolbar)
                .AddAdditionalValueIfNotEmpty("Rows", Rows)
                .TemplateHint = DataType.Html.ToString();

            AddConfigToMetadata(context.DisplayMetadata);
        }

        /// <summary>
        /// The toolbars to display. A default selection is used if this is not specified.
        /// </summary>
        public ICollection<HtmlToolbarPreset> Toolbars { get; private set; }

        /// <summary>
        /// When using HtmlToolbarPReset.Custom, use this to secify which buttons to include in
        /// the custom toolbar, e.g. "undo redo | bold italic underline | link unlink". Available
        /// buttons are listed in the TinyMCE docs: https://www.tinymce.com/docs/advanced/editor-control-identifiers/#toolbarcontrols
        /// </summary>
        public string CustomToolbar { get; set; }

        /// <summary>
        /// The number of visible lines of text in the text editor. Defaults
        /// to 20.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// A type to use to determine any additional configuration options 
        /// to apply to the html editor. This should be a class that inherits 
        /// from IHtmlEditorConfigSource, which provides a .net code generated set 
        /// of options.
        /// </summary>
        public Type ConfigSource { get; set; }

        /// <summary>
        /// The path the to a json config file which contains setting for the html 
        /// editor. This should be relative e.g. "/content/html-editor-config.json". 
        /// </summary>
        public string ConfigFilePath { get; set; }

        private void AddConfigToMetadata(DisplayMetadata modelMetaData)
        {
            if (!string.IsNullOrEmpty(ConfigFilePath))
            {
                ValidateFilePathSource();

                modelMetaData.AdditionalValues.Add("ConfigPath", ConfigFilePath);
            }

            if (ConfigSource == null) return;

            if (ConfigSource.GetConstructor(Type.EmptyTypes) == null)
            {
                var msg = $"ConfigSource type '{ConfigSource.FullName}'does not have a public parameterless constructor.";
                throw new InvalidOperationException(msg);
            }

            var source = Activator.CreateInstance(ConfigSource);

            if (source is IHtmlEditorConfigSource configSource)
            {
                modelMetaData.AdditionalValues.Add("Options", configSource.Create());
            } else
            {
                var msg = $"ConfigSource type '{ConfigSource.FullName}' is not a valid config type. Valid types inherit from {typeof(IHtmlEditorConfigSource).Name}";
                throw new InvalidOperationException(msg);
            }
        }

        private void ValidateFilePathSource()
        {
            if (string.IsNullOrWhiteSpace(ConfigFilePath))
            {
                var msg = $"{nameof(ConfigFilePath)} does not define a Path.";
                throw new Exception(msg);
            }

            if (!Uri.IsWellFormedUriString(ConfigFilePath, UriKind.Relative))
            {
                var msg = $"{nameof(ConfigFilePath)} is not a relative path. Only relative path types are supported.";
                throw new Exception(msg);
            }
        }
    }
}
