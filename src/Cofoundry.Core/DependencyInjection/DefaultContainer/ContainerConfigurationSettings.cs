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
    public class ContainerConfigurationHelper : IContainerConfigurationHelper
    {
        private readonly IConfiguration _configuration;

        public ContainerConfigurationHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Extracts the value with the specified key and converts it to type T.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="key">The configuration key for the value to convert.</param>
        /// <returns>The type to convert the value to.</returns>
        public T GetValue<T>(string key)
        {
            return _configuration.GetValue<T>(key);
        }

        /// <summary>
        /// Extracts the value with the specified key and converts it to type T.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="key">The configuration key for the value to convert.</param>
        /// <param name="defaultValue">The default value to use if no value is found.</param>
        /// <returns>The type to convert the value to.</returns>
        public T GetValue<T>(string key, T defaultValue)
        {
            return _configuration.GetValue<T>(key, defaultValue);
        }
    }
}
