using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

namespace Cofoundry.Web.Internal;

/// <summary>
/// Default implementation of <see cref="IApiResponseHelper"/>.
/// </summary>
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

    /// <inheritdoc/>
    public JsonResult SimpleQueryResponse<T>(T result)
    {
        var response = new ApiResponseHelperResult<T>()
        {
            Data = result
        };

        var jsonResult = CreateJsonResult(response);

        if (result == null)
        {
            jsonResult.StatusCode = 404;
        }

        return jsonResult;
    }

    /// <inheritdoc/>
    public JsonResult SimpleQueryResponse<T>(IEnumerable<ValidationError> validationErrors, T result)
    {
        var errors = FormatValidationErrors(validationErrors);
        var response = new ApiResponseHelperResult<T>
        {
            Errors = errors,
            IsValid = errors.Count == 0,
            Data = result
        };

        var jsonResult = CreateJsonResult(response);

        if (!response.IsValid)
        {
            jsonResult.StatusCode = 400;
        }
        else if (result == null)
        {
            jsonResult.StatusCode = 404;
        }

        return jsonResult;
    }

    /// <inheritdoc/>
    public JsonResult SimpleCommandResponse<T>(IEnumerable<ValidationError> validationErrors, T returnData)
    {
        var errors = FormatValidationErrors(validationErrors);
        var response = new ApiResponseHelperResult<T>
        {
            Errors = errors,
            IsValid = errors.Count == 0,
            Data = returnData
        };

        return GetCommandResponse(response);
    }

    /// <inheritdoc/>
    public JsonResult SimpleCommandResponse(IEnumerable<ValidationError> validationErrors)
    {
        var errors = FormatValidationErrors(validationErrors);
        var response = new ApiResponseHelperResult
        {
            Errors = errors,
            IsValid = errors.Count == 0
        };

        return GetCommandResponse(response);
    }

    /// <inheritdoc/>
    public JsonResult NotPermittedResponse(NotPermittedException ex)
    {
        var response = new ApiResponseHelperResult
        {
            Errors = [new ValidationError(ex.Message)],
            IsValid = false
        };

        var jsonResult = CreateJsonResult(response, 403);

        return jsonResult;
    }

    /// <inheritdoc/>
    public Task<JsonResult> RunQueryAsync<TResult>(IQuery<TResult> query)
    {
        return RunWithResultAsync(() => _queryExecutor.ExecuteAsync(query));
    }

    /// <inheritdoc/>
    public async Task<JsonResult> RunCommandAsync<TCommand>(int id, IDelta<TCommand> delta)
        where TCommand : class, IPatchableByIdCommand
    {
        var query = new GetPatchableCommandByIdQuery<TCommand>(id);
        var command = await _queryExecutor.ExecuteAsync(query);
        EntityNotFoundException.ThrowIfNull(command, id);

        delta?.Patch(command);

        return await RunCommandAsync(command);
    }

    /// <inheritdoc/>
    public async Task<JsonResult> RunCommandAsync<TCommand>(IDelta<TCommand> delta)
        where TCommand : class, IPatchableCommand
    {
        var query = new GetPatchableCommandQuery<TCommand>();
        var command = await _queryExecutor.ExecuteAsync(query);
        EntityNotFoundException.ThrowIfNull(command);

        delta?.Patch(command);

        return await RunCommandAsync(command);
    }

    /// <inheritdoc/>
    public async Task<JsonResult> RunCommandAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        var errors = _commandValidationService.GetErrors(command).ToList();

        if (errors.Count == 0)
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

    /// <inheritdoc/>
    public async Task<JsonResult> RunAsync(Func<Task> action)
    {
        var errors = new List<ValidationError>();

        if (errors.Count == 0)
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

    /// <inheritdoc/>
    public async Task<JsonResult> RunWithResultAsync<TResult>(Func<Task<TResult>> functionToExecute)
    {
        var errors = new List<ValidationError>();
        // null always allowed for a failed response despite nullability of TResult

        if (errors.Count == 0)
        {
            try
            {
                var result = await functionToExecute();
                return SimpleQueryResponse(errors, result);
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

        return SimpleQueryResponse(errors);
    }

    /// <inheritdoc/>
    public async Task<IActionResult?> RunWithActionResultAsync<TActionResult>(Func<Task<TActionResult>> functionToExecute)
        where TActionResult : IActionResult
    {
        var errors = new List<ValidationError>();
        IActionResult? result = null;

        if (errors.Count == 0)
        {
            try
            {
                result = await functionToExecute();
            }
            catch (ValidationException ex)
            {
                AddValidationExceptionToErrorList(ex, errors);
                return SimpleCommandResponse(errors);
            }
            catch (NotPermittedException ex)
            {
                return NotPermittedResponse(ex);
            }
        }

        return result;
    }

    private JsonResult GetCommandResponse<T>(T response) where T : ApiResponseHelperResult
    {
        var jsonResult = CreateJsonResult(response);

        if (!response.IsValid)
        {
            jsonResult.StatusCode = 400;
        }

        return jsonResult;
    }

    private static IReadOnlyCollection<ValidationError> FormatValidationErrors(IEnumerable<ValidationError> validationErrors)
    {
        if (validationErrors == null)
        {
            return Array.Empty<ValidationError>();
        }

        // De-dup and order by prop name.
        return validationErrors
            .GroupBy(e =>
            {
                var propKey = string.Empty;
                if (e.Properties != null)
                {
                    propKey = string.Join("+", e.Properties);
                }

                return new { e.Message, propKey };
            })
            .OrderBy(g => g.Key.propKey)
            .Select(g => g.First())
            .ToArray();

    }

    private static object? GetCommandOutputValue<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        var property = typeof(TCommand)
            .GetTypeInfo()
            .GetProperties()
            .SingleOrDefault(p => p.IsDefined(typeof(OutputValueAttribute)));

        if (property == null)
        {
            return null;
        }

        return property.GetValue(command);
    }

    private static void AddValidationExceptionToErrorList(ValidationException ex, List<ValidationError> errors)
    {
        if (ex.ValidationResult is CompositeValidationResult compositeResult)
        {
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
                error = new ValidationError()
                {
                    Message = ex.Message
                };
            }

            if (ex is ValidationErrorException exceptionWithCode)
            {
                error.ErrorCode = exceptionWithCode.ErrorCode;
            }

            errors.Add(error);
        }
    }

    private static ValidationError ToValidationError(ValidationResult result)
    {
        var error = new ValidationError()
        {
            Message = result.ErrorMessage ?? "Unknown Error",
            Properties = result.MemberNames.ToArray()
        };

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
