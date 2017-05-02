using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;
using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.WebApi
{
    /// <summary>
    /// Use this helper in a Web Api controller to make executing queries and command
    /// simpler and less repetitive. The helper handles validation error formatting, 
    /// permission errors and uses a standard formatting of the response.
    /// </summary>
    public class ApiResponseHelper : IApiResponseHelper
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IModelValidationService _commandValidationService;

        public ApiResponseHelper(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            IModelValidationService commandValidationService
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _commandValidationService = commandValidationService;
        }

        #endregion

        #region basic responses

        /// <summary>
        /// Formats the result of a query. Results are wrapped inside an object with a data property
        /// for consistency and prevent a vulnerability with return JSON arrays. 
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="controller">The Controller instance using the helper</param>
        /// <param name="result">The result to return</param>
        public IActionResult SimpleQueryResponse<T>(Controller controller, T result)
        {
            var response = new SimpleResponseData<T>() { Data = result };
            
            return controller.Ok(response);
        }

        /// <summary>
        /// Formats a command response wrapping it in a SimpleCommandResponse object and setting
        /// properties based on the presence of validation errors. This overload allows you to include
        /// extra response data
        /// </summary>
        /// <param name="controller">The Controller instance using the helper</param>
        /// <param name="validationErrors">Validation errors, if any, to be returned.</param>
        public IActionResult SimpleCommandResponse<T>(Controller controller, IEnumerable<ValidationError> validationErrors, T returnData)
        {
            var response = new SimpleCommandResponseData<T>();
            response.Errors = FormatValidationErrors(validationErrors);
            response.IsValid = !response.Errors.Any();
            response.Data = returnData;

            return GetCommandResponse(response, controller);
        }

        /// <summary>
        /// Formats a command response wrapping it in a SimpleCommandResponse object and setting
        /// properties based on the presence of validation errors.
        /// </summary>
        /// <param name="controller">The Controller instance using the helper</param>
        /// <param name="validationErrors">Validation errors, if any, to be returned.</param>
        public IActionResult SimpleCommandResponse(Controller controller, IEnumerable<ValidationError> validationErrors)
        {
            var response = new SimpleCommandResponseData();
            response.Errors = FormatValidationErrors(validationErrors);
            response.IsValid = !response.Errors.Any();

            return GetCommandResponse(response, controller);
        }

        /// <summary>
        /// Returns a formatted 403 error response using the message of the specified exception
        /// </summary>
        /// <param name="controller">The Controller instance using the helper</param>
        /// <param name="ex">The NotPermittedException to extract the message from</param>
        public IActionResult NotPermittedResponse(Controller controller, NotPermittedException ex)
        {
            var response = new SimpleCommandResponseData();
            response.Errors = new ValidationError[] { new ValidationError(ex.Message) };
            response.IsValid = false;

            return controller.StatusCode(403, response);
        }

        private IEnumerable<ValidationError> FormatValidationErrors(IEnumerable<ValidationError> validationErrors)
        {
            if (validationErrors == null) return Enumerable.Empty<ValidationError>();

            // De-dup and order by prop name.
            return validationErrors
                .GroupBy(e =>
                {
                    string propKey = string.Empty;
                    if (e.Properties != null)
                    {
                        propKey = string.Join("+", e.Properties);
                    }

                    return new { e.Message, propKey };
                })
                .OrderBy(g => g.Key.propKey)
                .Select(g => g.FirstOrDefault());

        }

        #endregion

        #region command helpers

        /// <summary>
        /// Executes a command in a "Patch" style, allowing for a partial update of a resource. In
        /// order to support this method, there must be a query handler defined that implements
        /// IQueryHandler&lt;GetByIdQuery&lt;TCommand&gt;&gt; so the full command object can be fecthed 
        /// prior to patching. Once patched and executed, a formatted IHttpActionResult is returned, 
        /// handling any validation errors and permission errors.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command to execute</typeparam>
        /// <param name="controller">The Controller instance using the helper</param>
        /// <param name="delta">The delta of the command to patch and execute</param>
        //public async Task<IActionResult> RunCommandAsync<TCommand>(Controller controller, int id, Delta<TCommand> delta) where TCommand : class, ICommand
        //{
        //    var command = await _queryExecutor.GetByIdAsync<TCommand>(id);

        //    if (delta != null)
        //    {
        //        delta.Patch(command);
        //    }

        //    return await RunCommandAsync(controller, command);
        //}

        /// <summary>
        /// Executes a command in a "Patch" style, allowing for a partial update of a resource. In
        /// order to support this method, there must be a query handler defined that implements
        /// IQueryHandler&lt;GetQuery&lt;TCommand&gt;&gt; so the full command object can be fecthed 
        /// prior to patching. Once patched and executed, a formatted IHttpActionResult is returned, 
        /// handling any validation errors and permission errors.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command to execute</typeparam>
        /// <param name="controller">The Controller instance using the helper</param>
        /// <param name="delta">The delta of the command to patch and execute</param>
        //public async Task<IActionResult> RunCommandAsync<TCommand>(Controller controller, Delta<TCommand> delta) where TCommand : class, ICommand
        //{
        //    var command = await _queryExecutor.GetAsync<TCommand>();

        //    if (delta != null)
        //    {
        //        delta.Patch(command);
        //    }


        //    return await RunCommandAsync(controller, command);
        //}

        /// <summary>
        /// Executes a command and returns a formatted IHttpActionResult, handling any validation 
        /// errors and permission errors. If the command has a property with the OutputValueAttribute
        /// the value is extracted and returned in the response.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command to execute</typeparam>
        /// <param name="controller">The Controller instance using the helper</param>
        /// <param name="command">The command to execute</param>
        public async Task<IActionResult> RunCommandAsync<TCommand>(Controller controller, TCommand command) where TCommand : ICommand
        {
            var errors = _commandValidationService.GetErrors(command).ToList();

            if (!errors.Any())
            {
                try
                {
                    await _commandExecutor.ExecuteAsync(command);
                }
                catch (ValidationException ex)
                {
                    AddValidationExceptionToErrorList(ex, errors);
                }
                catch (NotPermittedException ex)
                {
                    return NotPermittedResponse(controller, ex);
                }
            }

            var outputValue = GetCommandOutputValue(command);

            if (outputValue != null)
            {
                return SimpleCommandResponse(controller, errors, outputValue);
            }
            return SimpleCommandResponse(controller, errors);
        }

        /// <summary>
        /// Executes an action and returns a formatted IHttpActionResult, handling any validation 
        /// errors and permission errors.
        /// </summary>
        /// <param name="controller">The Controller instance using the helper</param>
        /// <param name="action">The action to execute</param>
        public async Task<IActionResult> RunAsync(Controller controller, Func<Task> action)
        {
            var errors = new List<ValidationError>();

            if (!errors.Any())
            {
                try
                {
                    await action();
                }
                catch (ValidationException ex)
                {
                    AddValidationExceptionToErrorList(ex, errors);
                }
                catch (NotPermittedException ex)
                {
                    return NotPermittedResponse(controller, ex);
                }
            }

            return SimpleCommandResponse(controller, errors);
        }

        /// <summary>
        /// Executes a function and returns a formatted IHttpActionResult, handling any validation 
        /// and permission errors. The result of the function is returned in the response data.
        /// </summary>
        /// <typeparam name="TResult">Type of result returned from the function</typeparam>
        /// <param name="controller">The Controller instance using the helper</param>
        /// <param name="function">The function to execute</param>
        public async Task<IActionResult> RunWithResultAsync<TResult>(Controller controller, Func<Task<TResult>> task)
        {
            var errors = new List<ValidationError>();
            TResult result = default(TResult);

            if (!errors.Any())
            {
                try
                {
                    result = await task();
                }
                catch (ValidationException ex)
                {
                    AddValidationExceptionToErrorList(ex, errors);
                }
                catch (NotPermittedException ex)
                {
                    return NotPermittedResponse(controller, ex);
                }
            }

            return SimpleCommandResponse(controller, errors, result);
        }

        #region private helpers

        private IActionResult GetCommandResponse<T>(T response, Controller controller) where T : SimpleCommandResponseData
        {
            if (!response.IsValid)
            {
                return controller.BadRequest(response);
            }

            return controller.Ok(response);
        }

        private object GetCommandOutputValue<TCommand>(TCommand command) where TCommand : ICommand
        {
            var property = typeof(TCommand).GetProperties()
                .SingleOrDefault(p => Attribute.IsDefined(p, typeof(OutputValueAttribute)));

            if (property == null) return null;

            return property.GetValue(command);
        }

        private void AddValidationExceptionToErrorList(ValidationException ex, List<ValidationError> errors)
        {
            if (ex.ValidationResult is CompositeValidationResult)
            {
                var compositeResult = (CompositeValidationResult)ex.ValidationResult;

                foreach (var result in compositeResult.Results)
                {
                    errors.Add(ToValidationError(result));
                }
            }
            else if (ex.ValidationResult != null)
            {
                errors.Add(ToValidationError(ex.ValidationResult));
            }
            else
            {
                var error = new ValidationError();
                error.Message = ex.Message;
                errors.Add(error);
            }
        }

        private ValidationError ToValidationError(ValidationResult result)
        {
            var error = new ValidationError();

            error.Message = result.ErrorMessage;
            error.Properties = result.MemberNames.ToArray();

            return error;
        }
        
        #endregion

        #endregion
    }
}
