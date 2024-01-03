using System.Reflection;

namespace Cofoundry.Core.Reflection.Internal;

public class MethodReferenceHelper
{
    /// <summary>
    /// Used in various places to get a MethodInfo reference for a private 
    /// non-static method, typicallly used for dynamically invoking a generic
    /// method with a runtime type reference. If the method could not be found 
    /// then an <see cref="Exception"/> is thrown.
    /// </summary>
    /// <typeparam name="TContainerClass">The type containing the method.</typeparam>
    public static MethodInfo GetPrivateInstanceMethod<TContainerClass>(string methodName)
        where TContainerClass : class
    {
        var methodInfo = typeof(TContainerClass).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

        if (methodInfo == null)
        {
            throw new Exception($"Could not find method {methodName} on type {typeof(TContainerClass).FullName}");
        }

        return methodInfo;
    }
}
