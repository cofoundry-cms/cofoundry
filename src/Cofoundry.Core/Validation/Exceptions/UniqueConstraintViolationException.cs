using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Validation
{
    /// <summary>
    /// Exception to be used when an entity fails a uniqueness check
    /// </summary>
    public class UniqueConstraintViolationException : ValidationException
    {
        public UniqueConstraintViolationException()
        {
        }

        public UniqueConstraintViolationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new UniqueConstraintViolationException relating to a single a property
        /// </summary>
        /// <param name="message">The message to assign to the exception</param>
        /// <param name="property">The property that failed validation.</param>
        /// <param name="value">Optionally value of the object/properties that caused the attribute to trigger validation error.</param>
        public UniqueConstraintViolationException(string message, string property, object value = null)
            : base(GetValidationResult(message, new string[] { property }), null, value)
        {
        }

        /// <summary>
        /// Throws a UniqueConstraintViolationException if the specified entity is not null.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity to check</typeparam>
        /// <param name="e">The entity to check</param>
        /// <param name="property">The name of the property that violates the uniqueness contraint.</param>
        public static void ThrowIfExists<TEntity>(TEntity e, string property) where TEntity : class
        {
            if (e != null)
            {
                throw new UniqueConstraintViolationException<TEntity>(property);
            }
        }

        private static ValidationResult GetValidationResult(string message, IEnumerable<string> properties)
        {
            var vr = new ValidationResult(message, properties);
            return vr;
        }
    }

    /// <summary>
    /// Exception to be used when an entity fails a uniqueness check
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity which failed the uniquenss check.</typeparam>
    public class UniqueConstraintViolationException<TEntity> : UniqueConstraintViolationException where TEntity : class
    {
        private const string errorMessage = "Entity of type '{0}' failed a uniqueness check on property {1}";
        private const string errorMessageWithoutValue = errorMessage + ".";
        private const string errorMessageWithValue = errorMessage + " with value {2}.";

        public UniqueConstraintViolationException(string property)
            : base(FormatMessage(property, null), property)
        {
        }

        public UniqueConstraintViolationException(string property, object value = null)
            : base(FormatMessage(property, value), property, value)
        {
        }

        private static string FormatMessage(string property, object value)
        {
            if (value == null)
            {
                return string.Format(errorMessageWithoutValue, typeof(TEntity).Name, property);
            }
            else
            {
                return string.Format(errorMessageWithValue, typeof(TEntity).Name, property, value.ToString());
            }
        }
    }
}
