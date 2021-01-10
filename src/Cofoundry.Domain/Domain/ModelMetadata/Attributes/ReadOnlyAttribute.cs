using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// Indicates that a property should not be editable in the
    /// admin UI. Other annotations can still be used to indicate
    /// the type of editor to use to display the read-only value 
    /// e.g. [Html] would render the value as Html, and [Date] would
    /// format the value as a date without a time.
    /// </para>
    /// <para>
    /// This attribute only affects the display of the property in the
    /// admin panel, and values can still be updated programmatically.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ReadOnlyAttribute : Attribute, IMetadataAttribute
    {
        public void Process(DisplayMetadataProviderContext context)
        {
            context
                .DisplayMetadata
                .AdditionalValues.Add("readonly", true)
                ;
        }
    }
}
