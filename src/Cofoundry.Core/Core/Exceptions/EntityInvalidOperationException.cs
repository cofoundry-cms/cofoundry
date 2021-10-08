using System;
using System.Linq.Expressions;

namespace Cofoundry.Core
{
    /// <summary>
    /// <para>
    /// Thrown when a method is invoked on an entity, but the entity is in 
    /// an invalid state.
    /// </para>
    /// <para>
    /// Use the generic version when throwing the exception, but you may
    /// catch the non-generic version if you don't want to catch the exception 
    /// for a specific entity.
    /// </para>
    /// </summary>
    public class EntityInvalidOperationException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInvalidOperationException"/>
        /// class using the default error message.
        /// </summary>
        public EntityInvalidOperationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInvalidOperationException"/>
        /// class using a specified error message.
        /// </summary>
        /// <param name="message">Message to use, or pass <see langword="null"/> to use the default.</param>
        public EntityInvalidOperationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInvalidOperationException"/> class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A specified message that states the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a 
        /// <see langword="null"/> reference if no inner exception is specified.
        /// </param>
        public EntityInvalidOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Throw an <see cref="EntityInvalidOperationException"/> if the specified entity 
        /// property is <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The type of entity that is in an invalid state. If the entity is <see langword="null"/> then an
        /// <see cref="EntityNotFoundException"/> is thrown instead.
        /// </typeparam>
        /// <param name="entity">The entity to check for a <see langword="null"/> reference.</param>
        /// <param name="memberSelector">A selector that targets the member (property or field) to check for <see langword="null"/>.</param>
        public static void ThrowIfNull<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> memberSelector) where TEntity : class
        {
            var memberExpression = memberSelector?.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException($"{nameof(memberSelector)} must be a property or field reference.", nameof(memberSelector));
            }

            if (entity == null)
            {
                throw new EntityNotFoundException<TEntity>();
            }

            var value = memberSelector.Compile().Invoke(entity);
            if (value == null)
            {
                throw new EntityInvalidOperationException<TEntity>(memberExpression.Member.Name, null);
            }
        }
    }

    /// <summary>
    /// <para>
    /// Thrown when a method is invoked on an entity, but the entity is in 
    /// an invalid state.
    /// </para>
    /// <para>
    /// This is a more specific verison of <see cref="InvalidOperationException"/>
    /// that can contain more information.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The type of entity that is in an invalid state.</typeparam>
    public class EntityInvalidOperationException<TEntity> : EntityInvalidOperationException where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInvalidOperationException"/>
        /// class using the default error message.
        /// </summary>
        public EntityInvalidOperationException()
            : base(FormatMessage(null, null))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInvalidOperationException"/>
        /// class using a specified error message.
        /// </summary>
        /// <param name="message">Message to use, or pass <see langword="null"/> to use the default.</param>
        public EntityInvalidOperationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInvalidOperationException"/> class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A specified message that states the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a 
        /// <see langword="null"/> reference if no inner exception is specified.
        /// </param>
        public EntityInvalidOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInvalidOperationException"/> class with
        /// a reference to the specific property or member and value.
        /// </summary>
        /// <param name="memberName">The name of the property or member in the unexpected state.</param>
        /// <param name="value">The unexpected value of the member.</param>
        public EntityInvalidOperationException(string memberName, object value)
            : base(FormatMessage(memberName, value))
        {
        }

        private static string FormatMessage(string memberName, object value)
        {
            var message = $"{typeof(TEntity).Name} is not in the expected state.";

            if (memberName != null)
            {
                message += $" The value of {memberName} should not be {value}";
            }

            return message;
        }
    }
}
