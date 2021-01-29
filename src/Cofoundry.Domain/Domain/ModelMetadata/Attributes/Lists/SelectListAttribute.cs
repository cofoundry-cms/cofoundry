using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this to decorate a collection attribute to indicate it should be
    /// rendered as a select list (drop down list) in the admin UI. The property should 
    /// use the same type as the associated option values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SelectListAttribute : Attribute, IMetadataAttribute
    {
        /// <summary>
        /// Use this to decorate a collection property to indicate it should be
        /// rendered as a select list (drop down list) in the admin UI. The collection type should 
        /// use the same type as the associated option values.
        /// </summary>
        /// <param name="optionSourceType">
        /// A type to use to determine the options available for the 
        /// property. This could be an Enum type, or a class that inherits 
        /// from either IListOptionSource (a static set of options) or 
        /// IListOptionApiSource (options generated from an api request).
        /// </param>
        public SelectListAttribute(Type optionSourceType)
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
        /// The text to display when the value is not set or has not 
        /// been selected yet.
        /// </summary>
        public string DefaultItemText { get; set; }

        public void Process(DisplayMetadataProviderContext context)
        {
            var modelMetaData = context.DisplayMetadata;
            modelMetaData.TemplateHint = "Dropdown";
            modelMetaData.AddAdditionalValueIfNotEmpty("DefaultItemText", DefaultItemText);

            ListOptionSourceMetadataHelper.AddToMetadata(modelMetaData, OptionSource);
        }
    }
}
