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
        private const string errorMessage = "Entity of type '{0}' failed a uniqueness check on property {1}.";

        public object Id { get; private set; }

        public UniqueConstraintViolationException(object id)
            : base(FormatMessage(id))
        {
            Id = id;
        }


        private static string FormatMessage(object id)
        {
            return string.Format(errorMessage, typeof(TEntity), id);
        }
    }
}
