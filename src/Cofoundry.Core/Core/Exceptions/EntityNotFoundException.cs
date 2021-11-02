using System;

namespace Cofoundry.Core
{
    /// <summary>
    /// <para>
    /// Exception to be used when an entity cannot be found but is required.
    /// </para>
    /// <para>
    /// Use the generic version when throwing the exception, but you may
    /// catch the non-generic version if you don't want to catch the exception 
    /// for a specific entity.
    /// </para>
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        private const string DEFAULT_MESSAGE = "An entity was required but could not be found.";

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/>
        /// class using the default error message.
        /// </summary>
        public EntityNotFoundException()
            : base(DEFAULT_MESSAGE)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/>
        /// class using a specified error message.
        /// </summary>
        /// <param name="message">Message to use, or pass <see langword="null"/> to use the default.</param>
        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A specified message that states the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a 
        /// <see langword="null"/> reference if no inner exception is specified.
        /// </param>
        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Throw an <see cref="EntityNotFoundException"/> if the specified entity 
        /// is <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity to check.</typeparam>
        /// <param name="entity">The entity to check for a <see langword="null"/> reference.</param>
        /// <param name="id">The unique identifier for the entity that could not be found.</param>
        public static void ThrowIfNull<TEntity>(TEntity entity, object id)
        {
            if (entity == null)
            {
                throw new EntityNotFoundException<TEntity>(null, id);
            }
        }
    }

    /// <summary>
    /// Exception to be used when an entity cannot be found but is required.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity which could not be found.</typeparam>
    public class EntityNotFoundException<TEntity> : EntityNotFoundException
    {
        private const string DEFAULT_MESSAGE = "An entity of type '{0}' was required but could not be found.";
        private const string MESSAGE_WITH_ID = "Entity of type '{0}' and identifier '{1}' could not be found.";

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/>
        /// class using the default error message.
        /// </summary>
        public EntityNotFoundException()
            : base(FormatDefaultMessage(null))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/>
        /// class using a specified error message.
        /// </summary>
        /// <param name="message">
        /// Message to use, or pass <see langword="null"/> to use the default. If using
        /// a custom message then you can use the {0} formatting token which will be
        /// replaced with the <see cref="TEntity"/> type name.
        /// </param>
        public EntityNotFoundException(string message)
            : base(FormatDefaultMessage(message))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// Message to use, or pass <see langword="null"/> to use the default. If using
        /// a custom message then you can use the {0} formatting token which will be
        /// replaced with the <see cref="TEntity"/> type name.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a 
        /// <see langword="null"/> reference if no inner exception is specified.
        /// </param>
        public EntityNotFoundException(string message, Exception innerException)
            : base(FormatDefaultMessage(message), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/>
        /// class using a specified error message and entity identifier. If 
        /// <paramref name="message"/> is null then a default message will be used.
        /// </summary>
        /// <param name="message">
        /// Message to use, or pass <see langword="null"/> to use the default. If using
        /// a custom message then there are two formatting tokens available: {0} for
        /// the <see cref="TEntity"/> type name and {1} for the <paramref name="id"/>.
        /// </param>
        /// <param name="id">The id of the entity that could not be found.</param>
        public EntityNotFoundException(string message, object id)
            : base(FormatMessageWithId(message, id))
        {
            Id = id;
        }

        /// <summary>
        /// The id of the entity that could not be found.
        /// </summary>
        public object Id { get; private set; }

        private static string FormatDefaultMessage(string message)
        {
            return string.Format(message ?? DEFAULT_MESSAGE, typeof(TEntity));
        }

        private static string FormatMessageWithId(string message, object id)
        {
            return string.Format(message ?? MESSAGE_WITH_ID, typeof(TEntity), id);
        }
    }
}
