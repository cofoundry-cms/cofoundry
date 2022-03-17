using Cofoundry.Core.ExecutionDurationRandomizer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Web;

/// <summary>
/// <para>
/// The task duration randomizer feature prevents the action completing before a random duration 
/// has elapsed by padding the execution time using <see cref="Task.Delay"/>. This can help mitigate 
/// against time-based enumeration attacks by extending the exection duration beyond the expected 
/// bounds of the action completion time. For example, this could be used to mitigate harvesting 
/// of valid usernames from login or forgot password pages by measuring the response times.
/// </para>
/// <para>
/// This attribute can be used on a controller action to apply the randomized duration feature to the
/// execution of the action.
/// </para>
/// </summary>
public class RandomDurationAttribute : ActionFilterAttribute
{
    /// <param name="minDurationInMilliseconds">
    /// The inclusive lower bound of the randomized task duration, measured in 
    /// milliseconds (1000ms = 1s).
    /// </param>
    /// <param name="maxDurationInMilliseconds">
    /// <summary>
    /// The inclusive upper bound of the randomized task duration, measured in 
    /// milliseconds (1000ms = 1s). Set to zero or null to disable the feature.
    /// </param>
    public RandomDurationAttribute(
        int minDurationInMilliseconds,
        int maxDurationInMilliseconds
        )
    {
        MinDurationInMilliseconds = minDurationInMilliseconds;
        MaxDurationInMilliseconds = maxDurationInMilliseconds;
    }

    /// <summary>
    /// The inclusive lower bound of the randomized task duration, measured in 
    /// milliseconds (1000ms = 1s).
    /// </summary>
    public int MinDurationInMilliseconds { get; }

    /// <summary>
    /// The inclusive upper bound of the randomized task duration, measured in 
    /// milliseconds (1000ms = 1s). Set to zero or null to disable the feature.
    /// </summary>
    public int MaxDurationInMilliseconds { get; }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var service = context.HttpContext.RequestServices.GetRequiredService<IExecutionDurationRandomizerScopeManager>();

        await using (service.Create(MinDurationInMilliseconds, MaxDurationInMilliseconds))
        {
            await base.OnActionExecutionAsync(context, next);
        }
    }
}
