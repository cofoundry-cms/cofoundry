using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Cofoundry.Web.Internal
{
    /// <inheritdoc/>
    public class ApiResponseHelper : IApiResponseHelper
    {
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

        public JsonResult SimpleCommandResponse<T>(IEnumerable<ValidationError> validationErrors, T returnData)
        {
            var response = new SimpleCommandResponseData<T>();
            response.Errors = FormatValidationErrors(validationErrors);
            response.IsValid = !response.Errors.Any();
            response.Data = returnData;

            return GetCommandResponse(response);
        }

        public JsonResult SimpleCommandResponse(IEnumerable<ValidationError> validationErrors)
        {
            var response = new SimpleCommandResponseData();
            response.Errors = FormatValidationErrors(validationErrors);
            response.IsValid = !response.Errors.Any();

            return GetCommandResponse(response);
        }

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

        public async Task<JsonResult> RunCommandAsync<TCommand>(int id, IDelta<TCommand> delta) 
            where TCommand : class, IPatchableByIdCommand
        {
            var query = new GetPatchableCommandByIdQuery<TCommand>(id);
            var command = await _queryExecutor.ExecuteAsync(query);

            if (delta != null)
            {
                delta.Patch(command);
            }

            return await RunCommandAsync(command);
        }

        public async Task<JsonResult> RunCommandAsync<TCommand>(IDelta<TCommand> delta) 
            where TCommand : class, IPatchableCommand
        {
            var query = new GetPatchableCommandQuery<TCommand>();
            var command = await _queryExecutor.ExecuteAsync(query);

            if (delta != null)
            {
                delta.Patch(command);
            }

            return await RunCommandAsync(command);
        }

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
    }
}
