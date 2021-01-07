using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Base functionality for the various date and time attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class DateTimeAttributeBase : ValidationAttribute, IMetadataAttribute
    {
        private readonly string _templateHint;
        private readonly string _minMaxFormat;

        public DateTimeAttributeBase(string templateHint, string minMaxFormat)
        {
            _templateHint = templateHint;
            _minMaxFormat = minMaxFormat;
            ErrorMessage = "{0} is not in a valid range.";
        }

        public virtual void Process(DisplayMetadataProviderContext context)
        {
            var modelMetaData = context.DisplayMetadata;
            modelMetaData.TemplateHint = _templateHint;

            context.DisplayMetadata
                .AddAdditionalValueIfNotEmpty(MinMetaDataProperty, Min)
                .AddAdditionalValueIfNotEmpty(MaxMetaDataProperty, Max)
                ;
        }

        protected virtual string MinMetaDataProperty { get; } = "Min";
        protected virtual string MaxMetaDataProperty { get; } = "Max";

        public abstract string Min { get; set; }

        public abstract string Max { get; set; }

        protected virtual DateTime? MinDate { get; set; }

        protected virtual DateTime? MaxDate { get; set; }

        protected string FormatDate(DateTime? date)
        {
            if (!date.HasValue) return null;
            return date.Value.ToString(_minMaxFormat);
        }

        protected static DateTime? ParseDate(string property, string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            if (!DateTime.TryParse(value, out DateTime result))
            {
                throw new InvalidOperationException($"{property} is not a valid date value: {value}");
            }

            return result;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || !MinDate.HasValue || !MaxDate.HasValue)
            {
                return ValidationResult.Success;
            }

            var parsed = ParseValueForValidation(value);

            if (!parsed.HasValue)
            {
                return ValidationResult.Success;
            }

            if ((MinDate.HasValue && parsed.Value < MinDate)
                || (MaxDate.HasValue && parsed.Value > MaxDate))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new string[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Casts or parses the object supplied by the IsValid method to
        /// a datetime so it can be validated.
        /// </summary>
        private DateTime? ParseValueForValidation(object value)
        {
            if (value == null) return null;

            DateTime parsed;

            if (value is DateTimeOffset offset)
            {
                parsed = offset.UtcDateTime;
            }
            else if (value is DateTime dateTime)
            {
                parsed = dateTime;
            }
            else if (value is string s && string.IsNullOrWhiteSpace(s))
            {
                return null;
            }
            else if (!DateTime.TryParse(
                value.ToString(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                out parsed))
            {
                throw new InvalidOperationException($"{value.GetType()} could not be converted to a DateTime for validation.");
            }

            return parsed;
        }
    }
}
