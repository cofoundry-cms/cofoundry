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
        public EntityInvalidOperationException()
            : base()
        {
        }

        public EntityInvalidOperationException(string message)
            : base(message)
        {
        }

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
            MemberExpression? memberExpression = null;

            if (memberSelector.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = memberSelector as MemberExpression;
            }

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
        public EntityInvalidOperationException()
            : base(FormatMessage(null, null))
        {
        }

        public EntityInvalidOperationException(string message)
            : base(message)
        {
        }

        public EntityInvalidOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

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
