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
using System.Reflection;
using Cofoundry.Core.Json;
using Newtonsoft.Json;

namespace Cofoundry.Web
{
    /// <summary>
    /// Use this helper in an API controller to make executing queries and command
    /// simpler and less repetitive. The helper handles validation error formatting, 
    /// permission errors and uses a standard formatting of the response in JSON.
    /// </summary>
    public class ApiResponseHelper : IApiResponseHelper
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IModelValidationService _commandValidationService;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public ApiResponseHelper(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            IModelValidationService commandValidationService,
            JsonSerializerSettings jsonSerializerSettings
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _commandValidationService = commandValidationService;
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        #endregion

        #region basic responses

        /// <summary>
        /// Formats the result of a query. Results are wrapped inside an object with a data property
        /// for consistency and prevent a vulnerability with return JSON arrays. If the result is
        /// null then a 404 response is returned.
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="result">The result to return</param>
        public JsonResult SimpleQueryResponse<T>(T result)
        {
            var response = new SimpleResponseData<T>() { Data = result };

            var jsonResult = CreateJsonResult(response);

            if (result == null)
            {
                jsonResult.StatusCode = 404;
            }

            return jsonResult;
        }

        /// <summary>
        /// Formats a command response wrapping it in a SimpleCommandResponse object and setting
        /// properties based on the presence of validation errors. This overload allows you to include
        /// extra response data
        /// </summary>
        /// <param name="validationErrors">Validation errors, if any, to be returned.</param>
        /// <param name="returnData">Data to return in the data property of the response object.</param>
        public JsonResult SimpleCommandResponse<T>(IEnumerable<ValidationError> validationErrors, T returnData)
        {
            var response = new SimpleCommandResponseData<T>();
            response.Errors = FormatValidationErrors(validationErrors);
            response.IsValid = !response.Errors.Any();
            response.Data = returnData;

            return GetCommandResponse(response);
        }

        /// <summary>
        /// Formats a command response wrapping it in a SimpleCommandResponse object and setting
        /// properties based on the presence of validation errors.
        /// </summary>
        /// <param name="validationErrors">Validation errors, if any, to be returned.</param>
        public JsonResult SimpleCommandResponse(IEnumerable<ValidationError> validationErrors)
        {
            var response = new SimpleCommandResponseData();
            response.Errors = FormatValidationErrors(validationErrors);
            response.IsValid = !response.Errors.Any();

            return GetCommandResponse(response);
        }

        /// <summary>
        /// Returns a formatted 403 error response using the message of the specified exception
        /// </summary>
        /// <param name="ex">The NotPermittedException to extract the message from</param>
        public JsonResult NotPermittedResponse(NotPermittedException ex)
        {
            var response = new SimpleCommandResponseData();
            response.Errors = new ValidationError[] { new ValidationError(ex.Message) };
            response.IsValid = false;

            var jsonResult = CreateJsonResult(response, 403);

            return jsonResult;
        }

        private ICollection<ValidationError> FormatValidationErrors(IEnumerable<ValidationError> validationErrors)
        {
            if (validationErrors == null) return Array.Empty<ValidationError>();

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
                .Select(g => g.FirstOrDefault())
                .ToArray();

        }

        #endregion

        #region command helpers

        /// <summary>
        /// Executes a command in a "Patch" style, allowing for a partial update of a resource. In
        /// order to support this method, there must be a query handler defined that implements
        /// IQueryHandler&lt;GetByIdQuery&lt;TCommand&gt;&gt; so the full command object can be fecthed 
        /// prior to patching. Once patched and executed, a formatted JsonResult is returned, 
        /// handling any validation errors and permission errors.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command to execute</typeparam>
        /// <param name="delta">The delta of the command to patch and execute</param>
        public async Task<JsonResult> RunCommandAsync<TCommand>(int id, IDelta<TCommand> delta) where TCommand : class, ICommand
        {
            var query = new GetUpdateCommandByIdQuery<TCommand>(id);
            var command = await _queryExecutor.ExecuteAsync(query);

            if (delta != null)
            {
                delta.Patch(command);
            }

            return await RunCommandAsync(command);
        }

        /// <summary>
        /// Executes a command in a "Patch" style, allowing for a partial update of a resource. In
        /// order to support this method, there must be a query handler defined that implements
        /// IQueryHandler&lt;GetQuery&lt;TCommand&gt;&gt; so the full command object can be fecthed 
        /// prior to patching. Once patched and executed, a formatted JsonResult is returned, 
        /// handling any validation errors and permission errors.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command to execute</typeparam>
        /// <param name="delta">The delta of the command to patch and execute</param>
        public async Task<JsonResult> RunCommandAsync<TCommand>(IDelta<TCommand> delta) where TCommand : class, ICommand
        {
            var query = new GetUpdateCommandQuery<TCommand>();
            var command = await _queryExecutor.ExecuteAsync(query);

            if (delta != null)
            {
                delta.Patch(command);
            }

            return await RunCommandAsync(command);
        }

        /// <summary>
        /// Executes a command and returns a formatted JsonResult, handling any validation 
        /// errors and permission errors. If the command has a property with the OutputValueAttribute
        /// the value is extracted and returned in the response.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command to execute</typeparam>
        /// <param name="command">The command to execute</param>
        public async Task<JsonResult> RunCommandAsync<TCommand>(TCommand command) where TCommand : ICommand
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
                    return NotPermittedResponse(ex);
                }
            }

            var outputValue = GetCommandOutputValue(command);

            if (outputValue != null)
            {
                return SimpleCommandResponse(errors, outputValue);
            }
            return SimpleCommandResponse(errors);
        }

        /// <summary>
        /// Executes an action and returns a formatted JsonResult, handling any validation 
        /// errors and permission errors.
        /// </summary>
        /// <param name="action">The action to execute</param>
        public async Task<JsonResult> RunAsync(Func<Task> action)
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
                    return NotPermittedResponse(ex);
                }
            }

            return SimpleCommandResponse(errors);
        }

        /// <summary>
        /// Executes a function and returns a formatted JsonResult, handling any validation 
        /// and permission errors. The result of the function is returned in the response data.
        /// </summary>
        /// <typeparam name="TResult">Type of result returned from the function</typeparam>
        /// <param name="functionToExecute">The function to execute</param>
        public async Task<JsonResult> RunWithResultAsync<TResult>(Func<Task<TResult>> functionToExecute)
        {
            var errors = new List<ValidationError>();
            TResult result = default(TResult);

            if (!errors.Any())
            {
                try
                {
                    result = await functionToExecute();
                }
                catch (ValidationException ex)
                {
                    AddValidationExceptionToErrorList(ex, errors);
                }
                catch (NotPermittedException ex)
                {
                    return NotPermittedResponse(ex);
                }
            }

            return SimpleCommandResponse(errors, result);
        }

        #region private helpers

        private JsonResult GetCommandResponse<T>(T response) where T : SimpleCommandResponseData
        {
            var jsonResult = CreateJsonResult(response);

            if (!response.IsValid)
            {
                jsonResult.StatusCode = 400;
            }

            return jsonResult;
        }

        private object GetCommandOutputValue<TCommand>(TCommand command) where TCommand : ICommand
        {
            var property = typeof(TCommand)
                .GetTypeInfo()
                .GetProperties()
                .SingleOrDefault(p => p.IsDefined(typeof(OutputValueAttribute)));

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
            else
            {
                ValidationError error;

                if (ex.ValidationResult != null)
                {
                    error = ToValidationError(ex.ValidationResult);
                }
                else
                {
                    error = new ValidationError();
                    error.Message = ex.Message;
                }

                if (ex is ValidationErrorException exceptionWithCode)
                {
                    error.ErrorCode = exceptionWithCode.ErrorCode;
                }

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

        private JsonResult CreateJsonResult(
            object response,
            int? statusCode = null
            )
        {
            var jsonResponse = new JsonResult(response, _jsonSerializerSettings);

            if (statusCode.HasValue)
            {
                jsonResponse.StatusCode = statusCode;
            }

            return jsonResponse;
        }

        #endregion

        #endregion
    }
}
