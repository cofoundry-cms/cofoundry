using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Validation
{
    /// <summary>
    /// Service for validating models using DataAnnotation validation.
    /// </summary>
    public interface IModelValidationService
    {
        /// <summary>
        /// Validates the specific model and throws an exception if it is null or 
        /// contains any invalid properties.
        /// </summary>
        /// <typeparam name="T">Type of the model to validate.</typeparam>
        /// <param name="commandToValidate">The command to validate.</param>
        void Validate<T>(T commandToValidate);

        /// <summary>
        /// Validates the specified model and returns a collection of any errors discovered in
        /// the validation process.
        /// </summary>
        /// <typeparam name="T">Type of model to validate.</typeparam>
        /// <param name="modelToValidate">The object to validate.</param>
        /// <returns>Enumerable collection of any errors found. Will be empty if the model is valid.</returns>
        IEnumerable<ValidationError> GetErrors<T>(T modelToValidate);

        /// <summary>
        /// Validates the specified models and returns a collection of any errors discovered in
        /// the validation process.
        /// </summary>
        /// <typeparam name="T">Type of model to validate.</typeparam>
        /// <param name="modelsToValidate">Collection of objects to validate.</param>
        /// <returns>Enumerable collection of any errors found. Will be empty if the model is valid.</returns>
        IEnumerable<ValidationError> GetErrors<T>(IEnumerable<T> modelsToValidate);
    }
}
