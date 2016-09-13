using Cofoundry.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Configuration
{
    public class InvalidConfigurationException : Exception
    {
        /// <summary>
        /// Creates a new PropertyValidationException relating to multiple properties
        /// </summary>
        /// <param name="message">The message to assign to the exception</param>
        /// <param name="properties">The properties that failed validation.</param>
        /// <param name="value">Optionally value of the object/properties that caused the attribute to trigger validation error.</param>
        public InvalidConfigurationException(string configName, IEnumerable<ValidationError> errors)
            : base(GetMessage(configName, errors))
        {
        }

        private static string GetMessage(string configName, IEnumerable<ValidationError> errors)
        {
            var errorMessage = errors?.Select(e => e.Message).FirstOrDefault();
            return configName + " configuration invalid: " + errorMessage;
        }
    }
}
