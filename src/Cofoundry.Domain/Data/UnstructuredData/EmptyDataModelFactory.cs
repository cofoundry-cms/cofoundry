namespace Cofoundry.Domain.Data;

/// <summary>
/// Default implementation of <see cref="IEmptyDataModelFactory"/>.
/// </summary>
public class EmptyDataModelFactory : IEmptyDataModelFactory
{
    ///<inheritdoc/>
    public TInterface Create<TInterface>(Type concreteType)
        where TInterface : class
    {
        object? value;

        try
        {
            value = Activator.CreateInstance(concreteType);
        }
        catch (MissingMethodException missingMethodException)
        {
            throw new Exception($"Unable to create an empty instance of type '{concreteType.FullName}', does it have a parameterless constructor? See inner exception for details.", missingMethodException);
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to create an empty instance of type '{concreteType.FullName}' because Activator.CreateInstance threw exception: {ex.Message}. See inner exception for details.", ex);
        }

        if (value == null)
        {
            throw new InvalidOperationException($"Unable to create an empty instance of type '{concreteType.FullName}' because Activator.CreateInstance returned null.");
        }

        var result = value as TInterface;
        if (result == null)
        {
            throw new InvalidOperationException($"Cannot create an empty instance of type '{concreteType.FullName}' because it does not implement '{typeof(TInterface).FullName}'.");
        }

        return result;
    }
}
