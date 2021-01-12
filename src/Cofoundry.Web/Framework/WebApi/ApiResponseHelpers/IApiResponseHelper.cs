using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Use this helper in an API controller to make executing queries and command
    /// simpler and less repetitive. The helper handles validation error formatting, 
    /// permission errors and uses a standard formatting of the response in JSON.
    /// </summary>
    public interface IApiResponseHelper
    {
        #region basic responses

        /// <summary>
        /// Formats the result of a query. Results are wrapped inside an object with a data property
        /// for consistency and prevent a vulnerability with return JSON arrays. If the result is
        /// null then a 404 response is returned.
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="result">The result to return</param>
        JsonResult SimpleQueryResponse<T>(T result);

        /// <summary>
        /// Formats a command response wrapping it in a SimpleCommandResponse object and setting
        /// properties based on the presence of validation errors. This overload allows you to include
        /// extra response data
        /// </summary>
        /// <param name="validationErrors">Validation errors, if any, to be returned.</param>
        /// <param name="returnData">Data to return in the data property of the response object.</param>
        JsonResult SimpleCommandResponse<T>(IEnumerable<ValidationError> validationErrors, T returnData);

        /// <summary>
        /// Formats a command response wrapping it in a SimpleCommandResponse object and setting
        /// properties based on the presence of validation errors.
        /// </summary>
        /// <param name="validationErrors">Validation errors, if any, to be returned.</param>
        JsonResult SimpleCommandResponse(IEnumerable<ValidationError> validationErrors);

        /// <summary>
        /// Returns a formatted 403 error response using the message of the specified exception
        /// </summary>
        /// <param name="ex">The NotPermittedException to extract the message from</param>
        JsonResult NotPermittedResponse(NotPermittedException ex);

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
        Task<JsonResult> RunCommandAsync<TCommand>(int id, IDelta<TCommand> delta) where TCommand : class, ICommand;

        /// <summary>
        /// Executes a command in a "Patch" style, allowing for a partial update of a resource. In
        /// order to support this method, there must be a query handler defined that implements
        /// IQueryHandler&lt;GetQuery&lt;TCommand&gt;&gt; so the full command object can be fecthed 
        /// prior to patching. Once patched and executed, a formatted JsonResult is returned, 
        /// handling any validation errors and permission errors.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command to execute</typeparam>
        /// <param name="delta">The delta of the command to patch and execute</param>
        Task<JsonResult> RunCommandAsync<TCommand>(IDelta<TCommand> delta) where TCommand : class, ICommand;

        /// <summary>
        /// Executes a command and returns a formatted JsonResult, handling any validation 
        /// errors and permission errors. If the command has a property with the OutputValueAttribute
        /// the value is extracted and returned in the response.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command to execute</typeparam>
        /// <param name="command">The command to execute</param>
        Task<JsonResult> RunCommandAsync<TCommand>(TCommand command) where TCommand : ICommand;

        /// <summary>
        /// Executes an action and returns a formatted JsonResult, handling any validation 
        /// errors and permission errors.
        /// </summary>
        /// <param name="action">The action to execute</param>
        Task<JsonResult> RunAsync(Func<Task> action);

        /// <summary>
        /// Executes a function and returns a formatted JsonResult, handling any validation 
        /// and permission errors. The result of the function is returned in the response data.
        /// </summary>
        /// <typeparam name="TResult">Type of result returned from the function</typeparam>
        /// <param name="functionToExecute">The function to execute</param>
        Task<JsonResult> RunWithResultAsync<TResult>(Func<Task<TResult>> functionToExecute);

        #endregion
    }
}