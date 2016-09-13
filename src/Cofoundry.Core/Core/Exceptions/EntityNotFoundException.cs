using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    /// <summary>
    /// Exception to be used when an entity cannot be found but is required.
    /// </summary>
    /// <remarks>
    /// Non-Generic class to make it easier to catch.
    /// </remarks>
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
        {
        }
        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// throw an EntityNotFoundException if the specified entity is null.
        /// </summary>
        /// <typeparam name="TEntity">Type fo the entity to check</typeparam>
        /// <param name="e">The entity to check for a null reference</param>
        /// <param name="id">The unique identifier for the entity that could not be found.</param>
        public static void ThrowIfNull<TEntity>(TEntity e, object id) where TEntity : class
        {
            if (e == null)
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

        public object Id { get; private set; }

        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(object id)
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
