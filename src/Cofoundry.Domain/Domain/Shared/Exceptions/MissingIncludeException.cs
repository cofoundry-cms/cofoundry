using System.Linq.Expressions;

namespace Cofoundry.Domain;

/// <summary>
/// Thrown when a related entity is required on an Entity Framework query
/// result, but has not been included. This is useful in mapping classes
/// where the query model passed into the mapper is required to have certain
/// data included.
/// </summary>
public class MissingIncludeException : Exception
{
    private const string DEFAULT_MESSAGE = "An entity relation was expected to be included in the Entity Framework query, but was found to be null.";

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingIncludeException"/>
    /// class using the default error message.
    /// </summary>
    public MissingIncludeException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingIncludeException"/>
    /// class using a specified error message.
    /// </summary>
    /// <param name="message">Message to use, or pass <see langword="null"/> to use the default.</param>
    public MissingIncludeException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingIncludeException"/> class with a specified error
    /// message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">A specified message that states the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a 
    /// <see langword="null"/> reference if no inner exception is specified.
    /// </param>
    public MissingIncludeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Throws an <see cref="MissingIncludeException"/> if the property
    /// specified in the <paramref name="includeEntitySelector"/> is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity to check for an included related entity.</typeparam>
    /// <typeparam name="TIncludedEntity">
    /// The type of the property selected in the <paramref name="includeEntitySelector"/> expression.
    /// </typeparam>
    /// <param name="entity">
    /// The entity containing the include property to check for. This should not be <see langword="null"/>.
    /// </param>
    /// <param name="includeEntitySelector">
    /// A selector that targets the entity relation that should not be <see langword="null"/>.
    /// </param>
    public static void ThrowIfNull<TEntity, TIncludedEntity>(TEntity entity, Expression<Func<TEntity, TIncludedEntity>> includeEntitySelector) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);

        var memberExpression = GetAsMemberExpression(includeEntitySelector, nameof(includeEntitySelector));
        var value = includeEntitySelector.Compile().Invoke(entity);

        if (value == null)
        {
            throw new MissingIncludeException<TEntity>(null, memberExpression.Member.Name);
        }
    }

    /// <summary>
    /// Throws an <see cref="MissingIncludeException"/> if the entity Id property specified in 
    /// <paramref name="includeEntityIdSelector"/> has a value but the entity specified in the 
    /// <paramref name="includeEntitySelector"/> is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity to check for an included related entity.</typeparam>
    /// <typeparam name="TIncludedEntity">
    /// The type of the property selected in the <paramref name="includeEntitySelector"/> expression.
    /// </typeparam>
    /// <typeparam name="TIncludedEntityId">
    /// The type of the property selected in the <paramref name="includeEntityIdSelector"/> expression.
    /// </typeparam>
    /// <param name="entity">
    /// The entity containing the include property to check for. This should not be null.
    /// </param>
    /// <param name="includeEntitySelector">
    /// A selector that targets the entity relation that should not be <see langword="null"/> if
    /// an id value is present.
    /// </param>
    /// <param name="includeEntityIdSelector">
    /// A selector that targets the id property associated with the entity relation that should be
    /// included. If the id property has a value then the entity relation will be validated to ensure
    /// it also have a value; otherwise the entity relation can be null.
    /// </param>
    public static void ThrowIfNull<TEntity, TIncludedEntity, TIncludedEntityId>(
        TEntity entity,
        Expression<Func<TEntity, TIncludedEntity>> includeEntitySelector,
        Expression<Func<TEntity, TIncludedEntityId?>> includeEntityIdSelector)
        where TEntity : class
        where TIncludedEntity : class
        where TIncludedEntityId : struct
    {
        ArgumentNullException.ThrowIfNull(entity);

        var includeMemberExpression = GetAsMemberExpression(includeEntitySelector, nameof(includeEntitySelector));
        var includeValue = includeEntitySelector.Compile().Invoke(entity);
        var idValue = includeEntityIdSelector.Compile().Invoke(entity);

        if (includeValue == null && idValue.Equals(default(TIncludedEntityId)))
        {
            throw new MissingIncludeException<TEntity>(null, includeMemberExpression.Member.Name);
        }
    }

    /// <summary>
    /// Throws an <see cref="MissingIncludeException"/> if the entity Id property specified in 
    /// <paramref name="includeEntityIdSelector"/> has a value but the entity specified in the 
    /// <paramref name="includeEntitySelector"/> is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity to check for an included related entity.</typeparam>
    /// <typeparam name="TIncludeProperty">
    /// The type of the property selected in the <paramref name="includeEntitySelector"/> expression.
    /// </typeparam>
    /// <typeparam name="TIncludedEntityId">
    /// The type of the property selected in the <paramref name="includeEntityIdSelector"/> expression.
    /// </typeparam>
    /// <param name="entity">
    /// The entity containing the include property to check for. This should not be null.
    /// </param>
    /// <param name="includeEntitySelector">
    /// A selector that targets the entity relation that should not be <see langword="null"/> if
    /// an id value is present.
    /// </param>
    /// <param name="includeEntityIdSelector">
    /// A selector that targets the id property associated with the entity relation that should be
    /// included. If the id property has a value then the entity relation will be validated to ensure
    /// it also have a value; otherwise the entity relation can be null.
    /// </param>
    public static void ThrowIfNull<TEntity, TIncludedEntity, TIncludedEntityId>(
        TEntity entity,
        Expression<Func<TEntity, TIncludedEntity>> includeMemberSelector,
        Expression<Func<TEntity, TIncludedEntityId>> includeMemberIdSelector)
        where TEntity : class
        where TIncludedEntity : class
        where TIncludedEntityId : IEquatable<TIncludedEntityId>
    {
        ArgumentNullException.ThrowIfNull(entity);

        var includeMemberExpression = GetAsMemberExpression(includeMemberSelector, nameof(includeMemberSelector));
        var includeValue = includeMemberSelector.Compile().Invoke(entity);
        var idValue = includeMemberIdSelector.Compile().Invoke(entity);

        if (includeValue == null && idValue.Equals(default(TIncludedEntityId)))
        {
            throw new MissingIncludeException<TEntity>(null, includeMemberExpression.Member.Name);
        }
    }

    private static MemberExpression GetAsMemberExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> selector, string propertyName) where TEntity : class
    {
        var memberExpression = selector?.Body as MemberExpression;

        if (memberExpression == null)
        {
            throw new ArgumentException($"{propertyName} must be a property or field reference.", propertyName);
        }

        return memberExpression;
    }
}

public class MissingIncludeException<TEntity> : MissingIncludeException where TEntity : class
{
    private const string DEFAULT_MESSAGE = "The '{1}' entity relation was expected to be included in the '{0}' Entity Framework query, but was found to be null.";
    private const string MESSAGE_WITH_MEMBERNAME = "An entity relation was expected to be included in the '{0}' Entity Framework query, but was found to be null.";

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingIncludeException"/>
    /// class using the default error message.
    /// </summary>
    public MissingIncludeException()
        : base(FormatMessage(null, null))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingIncludeException"/>
    /// class using a specified error message.
    /// </summary>
    /// <param name="message">Message to use, or pass <see langword="null"/> to use the default.</param>
    public MissingIncludeException(string message)
        : base(FormatMessage(message, null))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingIncludeException"/> class with a specified error
    /// message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">A specified message that states the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a 
    /// <see langword="null"/> reference if no inner exception is specified.
    /// </param>
    public MissingIncludeException(string message, Exception innerException)
        : base(FormatMessage(message, null), innerException)
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
    /// the <see cref="TEntity"/> type name and {1} for the <paramref name="memberName"/>.
    /// </param>
    /// <param name="memberName">The name of the property or member that is an include.</param>
    public MissingIncludeException(string message, string memberName)
        : base(FormatMessage(message, memberName))
    {
    }

    private static string FormatMessage(string message, string memberName)
    {
        if (memberName != null)
        {
            return string.Format(message ?? DEFAULT_MESSAGE, typeof(TEntity).Name);
        }

        return string.Format(message ?? MESSAGE_WITH_MEMBERNAME, typeof(TEntity).Name, memberName);
    }
}
