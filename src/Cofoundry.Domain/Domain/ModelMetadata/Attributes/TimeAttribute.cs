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
    /// Use this to decorate a TimeSpan or string property 
    /// and provide a UI hint to the admin interface to display a 
    /// time input field. By default the editor will show hours and 
    /// minutes, but you can allow editing of seconds by setting the 
    /// "Step" property to 1.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TimeAttribute : ValidationAttribute, IMetadataAttribute
    {
        public TimeAttribute()
        {
            ErrorMessage = "{0} is not in a valid range.";
        }

        public void Process(DisplayMetadataProviderContext context)
        {
            var modelMetaData = context.DisplayMetadata;
            modelMetaData.TemplateHint = "Time";
            var displayName = context.DisplayMetadata.DisplayName();

            context.DisplayMetadata
                .AddAdditionalValueIfNotEmpty("Step", Step)
                .AddAdditionalValueIfNotEmpty("Min", Min)
                .AddAdditionalValueIf(!string.IsNullOrWhiteSpace(Min), "MinValMsg", $"{displayName} cannot be earlier than {Min}")
                .AddAdditionalValueIfNotEmpty("Max", Max)
                .AddAdditionalValueIf(!string.IsNullOrWhiteSpace(Max), "MaxValMsg", $"{displayName} cannot be later than {Max}")
                ;
        }

        /// <summary>
        /// The inclusive minimum time allowed to be entered. The value must be
        /// in "hh:mm" or "hh:mm:ss" format.
        /// </summary>
        public string Min { get; set; }

        /// <summary>
        /// The inclusive maximum time allowed to be entered. The value must be
        /// in "hh:mm" or "hh:mm:ss" format.
        /// </summary>
        public string Max { get; set; }

        /// <summary>
        /// Maps to the "step" html attribute, and is the number of seconds to
        /// increment the input by. The browser default is 1 minute (60 seconds)
        /// and will not allow seconds to be input. If this value is set to less
        /// then 60 seconds, then seconds will be shown in the editor i.e. to make
        /// seconds editable, set this value to 1.
        /// </summary>
        public int Step { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            TimeSpan? minTime = ParseTime(Min, validationContext);
            TimeSpan? maxTime = ParseTime(Max, validationContext);

            if (value == null || !minTime.HasValue || !maxTime.HasValue)
            {
                return ValidationResult.Success;
            }

            var parsed = ParseValueForValidation(value);

            if (!parsed.HasValue)
            {
                return ValidationResult.Success;
            }

            if ((minTime.HasValue && parsed.Value < minTime)
                || (maxTime.HasValue && parsed.Value > maxTime))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new string[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }

        private TimeSpan? ParseTime(string timeAsString, ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(timeAsString)) return null;
            if (!TimeSpan.TryParse(timeAsString, out TimeSpan time))
            {
                throw new InvalidOperationException($"{validationContext.MemberName} is not a valid time value: {timeAsString}");
            }

            return time;
        }

        private TimeSpan? ParseValueForValidation(object value)
        {
            if (value == null) return null;

            TimeSpan parsed;

            if (value is TimeSpan timeSpan)
            {
                parsed = timeSpan;
            }
            else if (value is string s && string.IsNullOrWhiteSpace(s))
            {
                return null;
            }
            else if (!TimeSpan.TryParse(
                value.ToString(),
                out parsed))
            {
                throw new InvalidOperationException($"{value.GetType()} could not be converted to a TimeSpan for validation.");
            }

            return parsed;
        }
    }
}
