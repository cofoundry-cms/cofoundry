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
    /// a UI hint to the admin interface to display a date and time input field. The UI 
    /// picker and value is timezone insensitive i.e. UTC. For a timezone-sensitive value 
    /// use [DateAndTimeLocal].
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DateAndTimeAttribute : DateTimeAttributeBase
    {
        public DateAndTimeAttribute()
            : base("DateTime", "yyyy-MM-ddTHH:mm")
        {
        }

        /// <summary>
        /// Maps to the "step" html attribute, and is the number of seconds to
        /// increment the input by. The browser default is 1 minute (60 seconds)
        /// and will not allow seconds to be input. If this value is set to less
        /// then 60 seconds, then seconds will be shown in the editor i.e. to make
        /// seconds editable, set this value to 1.
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// The inclusive minimum date and time allowed to be entered. The value must be
        /// in "yyyy-mm-ddThh:mm" format.
        /// </summary>
        public override string Min { get => FormatDate(MinDate); set => MinDate = ParseDate(nameof(Min), value); }

        /// <summary>
        /// The inclusive maximum date and time allowed to be entered. The value must be
        /// in "yyyy-mm-ddThh:mm" format.
        /// </summary>
        public override string Max { get => FormatDate(MaxDate); set => MaxDate = ParseDate(nameof(Max), value); }

        public override void Process(DisplayMetadataProviderContext context)
        {
            base.Process(context);
            context.DisplayMetadata
                .AddAdditionalValueIfNotEmpty("Step", Step)
                ;
        }
    }
}
