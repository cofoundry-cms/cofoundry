using Cofoundry.Core.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public static class ITransationScopeExtensions
    {
        /// <summary>
        /// Completes the transaction scope only if the <paramref name="modelState"/> is
        /// valid, indicating that no handled or unhandled errors occurred udring execution.
        /// </summary>
        /// <param name="modelState">
        /// The <see cref="ModelStateDictionary"/> to check for validity, usually accessed via
        /// controller.ModelState or similar.
        /// </param>
        public static async Task CompleteIfValidAsync(this ITransactionScope scope, ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                await scope.CompleteAsync();
            }
        }
    }
}
