using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Core.Validation
{
    /// <summary>
    /// Exception to be used when an entity fails a uniqueness check.
    /// </summary>
    public class UniqueConstraintViolationException : ValidationException
    {
        private const string DEFAULT_MESSAGE = "A uniqueness check has failed.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueConstraintViolationException"/>
        /// class using the default error message.
        /// </summary>
        public UniqueConstraintViolationException()
            : base(DEFAULT_MESSAGE)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueConstraintViolationException"/>
        /// class using a specified error message.
        /// </summary>
        /// <param name="message">A specified message that states the error.</param>
        public UniqueConstraintViolationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueConstraintViolationException"/>
        /// class using a specified error message and a collection of inner exception instances.
        /// </summary>
        /// <param name="message">A specified message that states the error.</param>
        /// <param name="innerException">The collection of validation exceptions.</param>
        public UniqueConstraintViolationException(string message, Exception innerException)
            : base(message, innerException)
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
        /// Throws a <see cref="UniqueConstraintViolationException"/> if the specified entity is 
        /// not <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity to check.</typeparam>
        /// <param name="entity">The entity to check.</param>
        /// <param name="property">The name of the property that violates the uniqueness contraint.</param>
        public static void ThrowIfExists<TEntity>(TEntity entity, string property) where TEntity : class
        {
            if (entity != null)
            {
                throw new UniqueConstraintViolationException<TEntity>(null, property);
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
        private const string BASE_MESSAGE = "Entity of type '{0}' failed a uniqueness check";
        private const string MESSAGE_WITHOUT_PROPERTY = BASE_MESSAGE + ".";
        private const string MESSAGE_WITH_PROPERTY = BASE_MESSAGE + " on property {1}.";
        private const string MESSAGE_WITH_VPROPERTY_AND_VALUE = BASE_MESSAGE + " on property {1} with value {2}.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueConstraintViolationException"/>
        /// class using the default error message.
        /// </summary>
        public UniqueConstraintViolationException()
            : base(FormatMessage(null, null, null))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueConstraintViolationException"/>
        /// class using a specified error message.
        /// </summary>
        /// <param name="message">A specified message that states the error.</param>
        public UniqueConstraintViolationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueConstraintViolationException"/>
        /// class using a specified error message and a collection of inner exception instances.
        /// </summary>
        /// <param name="message">A specified message that states the error.</param>
        /// <param name="innerException">The collection of validation exceptions.</param>
        public UniqueConstraintViolationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a new <see cref="UniqueConstraintViolationException"/> relating to a single
        /// a property
        /// </summary>
        /// <param name="message">The message to assign to the exception.</param>
        /// <param name="property">The property that failed validation.</param>
        /// <param name="value">Optionally value of the object/properties that caused the attribute to trigger validation error.</param>
        public UniqueConstraintViolationException(string message, string property, object value = null)
            : base(FormatMessage(message, property, value), property, value)
        {
        }

        private static string FormatMessage(string message, string property, object value)
        {
            if (!string.IsNullOrWhiteSpace(message)) return message;
            if (string.IsNullOrWhiteSpace(property)) return MESSAGE_WITHOUT_PROPERTY;

            if (value == null)
            {
                return string.Format(MESSAGE_WITH_PROPERTY, typeof(TEntity).Name, property);
            }

            return string.Format(MESSAGE_WITH_VPROPERTY_AND_VALUE, typeof(TEntity).Name, property, value.ToString());
        }
    }
}
