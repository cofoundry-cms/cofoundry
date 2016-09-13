using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.OData;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;
using Cofoundry.Core;

namespace Cofoundry.Web.WebApi
{
    public class ApiResponseHelper
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

        public IHttpActionResult SimpleQueryResponse<T>(ApiController controller, T result)
        {
            var response = new SimpleResponseData<T>() { Data = result };
            return new OkNegotiatedContentResult<SimpleResponseData<T>>(response, controller);
        }

        public IHttpActionResult SimpleCommandResponse<T>(ApiController controller, IEnumerable<ValidationError> validationErrors, T returnData)
        {
            var response = new SimpleCommandResponseData<T>();
            response.Errors = FormatValidationErrors(validationErrors);
            response.IsValid = !response.Errors.Any();
            response.Data = returnData;

            var responseCode = response.IsValid ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            return new NegotiatedContentResult<SimpleCommandResponseData<T>>(responseCode, response, controller);
        }

        public IHttpActionResult SimpleCommandResponse(ApiController controller, IEnumerable<ValidationError> validationErrors)
        {
            var response = new SimpleCommandResponseData();
            response.Errors = FormatValidationErrors(validationErrors);
            response.IsValid = !response.Errors.Any();

            var responseCode = response.IsValid ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            return new NegotiatedContentResult<SimpleCommandResponseData>(responseCode, response, controller);
        }

        public IHttpActionResult NotPermittedResponse(ApiController controller, NotPermittedException ex)
        {
            var response = new SimpleCommandResponseData();
            response.Errors = new ValidationError[] { new ValidationError(ex.Message) };
            response.IsValid = false;

            return new NegotiatedContentResult<SimpleCommandResponseData>(HttpStatusCode.Forbidden, response, controller);
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

        public async Task<IHttpActionResult> RunCommandAsync<TCommand>(ApiController controller, int id, Delta<TCommand> delta) where TCommand : class, ICommand
        {
            var command = await _queryExecutor.GetByIdAsync<TCommand>(id);

            if (delta != null)
            {
                delta.Patch(command);
            }

            return await RunCommandAsync(controller, command);
        }

        public async Task<IHttpActionResult> RunCommandAsync<TCommand>(ApiController controller, Delta<TCommand> delta) where TCommand : class, ICommand
        {
            var command = await _queryExecutor.GetAsync<TCommand>();

            if (delta != null)
            {
                delta.Patch(command);
            }


            return await RunCommandAsync(controller, command);
        }

        public async Task<IHttpActionResult> RunCommandAsync<TCommand>(ApiController controller, TCommand command) where TCommand : ICommand
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

        #region private helpers

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
