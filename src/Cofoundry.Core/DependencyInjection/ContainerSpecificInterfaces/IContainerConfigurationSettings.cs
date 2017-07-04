using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.DependencyInjection
{
    /// <summary>
    /// Simple abstraction over configuration settings that gets exposed in
    /// an IContainerRegister instance.
    /// </summary>
    /// <remarks>
    /// This is a basic abstraction to reduce the surface area of IConfigurationRoot
    /// as we may need to alter the functionality here at a later date.
    /// </remarks>
    public interface IContainerConfigurationHelper
    {
        /// <summary>
        /// Extracts the value with the specified key and converts it to type T.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="key">The configuration key for the value to convert.</param>
        /// <returns>The type to convert the value to.</returns>
        T GetValue<T>(string key);

        /// <summary>
        /// Extracts the value with the specified key and converts it to type T.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="key">The configuration key for the value to convert.</param>
        /// <param name="defaultValue">The default value to use if no value is found.</param>
        /// <returns>The type to convert the value to.</returns>
        T GetValue<T>(string key, T defaultValue);
    }
}
