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
    public abstract class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }

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
        public static void ThrowIfNull<TEntity>(TEntity entity, object id) where TEntity : class
        {
            if (entity == null)
            {
                throw new EntityNotFoundException<TEntity>(id);
            }
        }
    }

    /// <summary>
    /// Exception to be used when an entity cannot be found but is required.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity which could not be found.</typeparam>
    public class EntityNotFoundException<TEntity> : EntityNotFoundException where TEntity : class
    {
        private const string errorMessage = "Entity of type '{0}' and identifier '{1}' could not be found.";

        /// <summary>
        /// The id of the entity that could not be found.
        /// </summary>
        public object Id { get; private set; }

        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public EntityNotFoundException(object id)
            : base(FormatMessage(id))
        {
            Id = id;
        }

        public EntityNotFoundException(object id, string message)
            : base(message)
        {
            Id = id;
        }

        private static string FormatMessage(object id)
        {
            return string.Format(errorMessage, typeof(TEntity), id);
        }
    }
}
