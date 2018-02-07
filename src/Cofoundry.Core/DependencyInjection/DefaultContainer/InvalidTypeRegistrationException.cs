using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.DependencyInjection
{
    /// <summary>
    /// Thrown when there is an invalid registration configuration
    /// for a specific type.
    /// </summary>
    public class InvalidTypeRegistrationException : Exception
    {
        public InvalidTypeRegistrationException(Type type)
            : base(MakeDefaultMessage(type))
        {
            this.RegisteredType = type;
        }

        public InvalidTypeRegistrationException(Type type, string message)
            : base(message)
        {
            this.RegisteredType = type;
        }

        public InvalidTypeRegistrationException(Type type, string message, Exception innerException)
            : base(message, innerException)
        {
            this.RegisteredType = type;
        }

        private static string MakeDefaultMessage(Type type)
        {
            const string DEFAULT_MESSAGE = "The configuration for type {0} is invalid.";

            return string.Format(DEFAULT_MESSAGE, type.FullName);
        }

        public Type RegisteredType { get; set; }
    }
}
