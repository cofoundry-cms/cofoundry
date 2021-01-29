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
    /// and value is timezone insensitive i.e. UTC, with the time value always set to 
    /// midnight UTC. For a timezone-sensitive value use [DateLocal].
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DateAttribute : DateTimeAttributeBase
    {
        public DateAttribute()
            : base("Date", "yyyy-MM-dd")
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
