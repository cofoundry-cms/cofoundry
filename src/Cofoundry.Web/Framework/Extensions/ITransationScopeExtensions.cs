using Cofoundry.Core.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cofoundry.Web;

/// <summary>
/// Extension methods for <see cref="ITransactionScope"/>.
/// </summary>
public static class ITransationScopeExtensions
{
    extension(ITransactionScope scope)
    {
        /// <summary>
        /// Completes the transaction scope only if the <paramref name="modelState"/> is
        /// valid, indicating that no handled or unhandled errors occurred udring execution.
        /// </summary>
        /// <param name="modelState">
        /// The <see cref="ModelStateDictionary"/> to check for validity, usually accessed via
        /// controller.ModelState or similar.
        /// </param>
        public async Task CompleteIfValidAsync(ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                await scope.CompleteAsync();
            }
        }
    }
}
