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
    /// Use this to decorate a DateTime, DateTimeOffset or string property and provide 
    /// a UI hint to the admin interface to display a date picker field. The UI picker 
    /// uses the web browser timezone offset, however the value is converted to UTC when 
    /// saving the value. The time is always set to midnight in the web browser timezone. 
    /// For a timezone-insensitive picker use [Date].
    /// </summary>
    /// <remarks>
    /// This replicates the behavior of the DateAttribute prior to Cofoundry 0.9
    /// and can be used for backwards compatibility.
    /// </remarks>
    public class DateLocalAttribute : DateTimeAttributeBase
    {
        public DateLocalAttribute()
            : base("DateLocal", "yyyy-MM-dd")
        {
        }

        /// <summary>
        /// The inclusive minimum date allowed to be entered. The value must be
        /// in "yyyy-mm-dd" format.
        /// </summary>
        public override string Min { get => FormatDate(MinDate); set => MinDate = ParseDate(nameof(Min), value); }

        /// <summary>
        /// The inclusive maximum date allowed to be entered. The value must be
        /// in "yyyy-mm-dd" format.
        /// </summary>
        public override string Max { get => FormatDate(MaxDate); set => MaxDate = ParseDate(nameof(Max), value); }
    }
}
