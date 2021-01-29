using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// Use this to decorate a collection property to indicate it should be
    /// rendered as a list of checkbox inputs in the admin UI. The collection type 
    /// should use the same type as the associated option values.
    /// </para>
    /// <para>
    /// A checkbox list allows multiple values to be selected and so the property 
    /// type should be a collection, and the collection type should use the same 
    /// type as the associated option values.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CheckboxListAttribute : Attribute, IMetadataAttribute
    {
        /// <summary>
        /// Use this to decorate a collection property to indicate it should be
        /// rendered as a list of checkbox inputs in the admin UI. The collection type 
        /// should use the same type as the associated option values.
        /// </summary>
        /// <param name="optionSourceType">
        /// A type to use to determine the options available for the 
        /// property. This could be an Enum type, or a class that inherits 
        /// from either IListOptionSource (a static set of options) or 
        /// IListOptionApiSource (options generated from an api request).
        /// </param>
        public CheckboxListAttribute(Type optionSourceType)
        {
            if (optionSourceType == null) throw new ArgumentNullException(nameof(optionSourceType));
            OptionSource = optionSourceType;
        }

        /// <summary>
        /// A type to use to determine the options available for the 
        /// property. This could be an Enum type, or a class that inherits 
        /// from either IListOptionSource (a static set of options) or 
        /// IListOptionApiSource (options generated from an api request).
        /// </summary>
        public Type OptionSource { get; private set; }

        /// <summary>
        /// The text to show in non-edit mode when no value is selected.
        /// </summary>
        public string NoValueText { get; set; }

        public void Process(DisplayMetadataProviderContext context)
        {
            var modelMetaData = context.DisplayMetadata;
            modelMetaData.TemplateHint = "CheckboxList";
            modelMetaData.AddAdditionalValueIfNotEmpty("NoValueText", NoValueText);

            ListOptionSourceMetadataHelper.AddToMetadata(modelMetaData, OptionSource);
        }
    }
}
