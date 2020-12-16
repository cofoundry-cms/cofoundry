using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Cofoundry.Core.Validation.Internal
{
    /// <summary>
    /// Service for validating models using DataAnnotation validation.
    /// </summary>
    public class ModelValidationService : IModelValidationService
    {
        /// <summary>
        /// Validates the specific model and throws an exception if it is null or 
        /// contains any invalid properties.
        /// </summary>
        /// <typeparam name="T">Type of the model to validate.</typeparam>
        /// <param name="modelToValidate">The command to validate.</param>
        public virtual void Validate<T>(T modelToValidate)
        {
            if (modelToValidate == null)
            {
                throw new ArgumentNullException("modelToValidate");
            }

            var cx = new ValidationContext(modelToValidate);
            Validator.ValidateObject(modelToValidate, cx, true);
        }

        /// <summary>
        /// Validates the specified model and returns a collection of any errors discovered in
        /// the validation process.
        /// </summary>
        /// <typeparam name="T">Type of model to validate.</typeparam>
        /// <param name="modelsToValidate">Collection of objects to validate.</param>
        /// <returns>Enumerable collection of any errors found. Will be empty if the model is valid.</returns>
        public virtual IEnumerable<ValidationError> GetErrors<T>(IEnumerable<T> modelsToValidate)
        {
            foreach (var model in modelsToValidate)
            {
                var errors = GetErrors(model);
                foreach (var error in errors)
                {
                    yield return error;
                }
            }
        }

        /// <summary>
        /// Validates the specified models and returns a collection of any errors discovered in
        /// the validation process.
        /// </summary>
        /// <typeparam name="T">Type of model to validate.</typeparam>
        /// <param name="modelToValidate">The object to validate.</param>
        /// <returns>Enumerable collection of any errors found. Will be empty if the model is valid.</returns>
        public virtual IEnumerable<ValidationError> GetErrors<T>(T modelToValidate)
        {
            if (modelToValidate == null)
            {
                throw new ArgumentNullException("modelToValidate");
            }

            var validationResults = new List<ValidationResult>();
            var cx = new ValidationContext(modelToValidate);
            Validator.TryValidateObject(modelToValidate, cx, validationResults, true);

            foreach (var result in validationResults)
            {
                if (result is CompositeValidationResult)
                {
                    var compositeResult = (CompositeValidationResult)result;
                    foreach (var childResult in compositeResult.Results)
                    {
                        yield return MapErrors(childResult);
                    }
                }
                else
                {
                    yield return MapErrors(result);
                }
            }
        }

        protected ValidationError MapErrors(ValidationResult result)
        {
            var error = new ValidationError();
            error.Message = result.ErrorMessage;
            error.Properties = result.MemberNames.ToArray();
            return error;
        }
    }
}
