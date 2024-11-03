An authorized task represents a single user-based operation that can be executed without being signed in. Task authorization is validated by a unique cryptographically secure token, often communicated via an out-of-band communication mechanism such as an email. 

To better explain this, let's use an example. Cofoundry includes two flows that are built on top of the authorized task framework: "User Account Recovery" and "User Account Verification". Let's look at a user account recovery (AKA "forgot password") flow in more detail:

1. A user starts the recovery process by entering their email address into a form
2. If the user account exists then an authorized task is initiated 
3. During initialization, a unique cryptographically secure token is generated
4. The token is added to a password reset URL and emailed to the user
5. When the user follows the link in their email, the token is validated before displaying a password reset form
6. When submitting the form, the task token is validated again before executing the password change
7. The task is marked as complete so that the token cannot be used again
8. Any other account recovery tokens the user has generated are invalidated

In the example above, the authorized task framework generates the token and tracks the task through to completion. Some other features of the framework include:

- **Expiry:** Tasks can be made to be valid for specific duration
- **Rate Limiting:** Tasks types can be rate limited to prevent abuse
- **Task data:** Additional data can be added to a task during initialization which may be used for additional validation or task completion

This flexibility is useful for several scenarios, but for rest of this document we'll use the task "managing user invites" as an example.

## Definition

To get started with authorized tasks we first need to define a task type. This is how we refer to groups of tasks of the same type:

```csharp
public using Cofoundry.Domain;

public class MemberInviteAuthorizedTaskType : IAuthorizedTaskTypeDefinition
{
    /// <summary>
    /// Convention is to use a public constant to make it
    /// easier to reference the identifying AuthorizedTaskTypeCode.
    /// </summary>
    public const string Code = "MEMINV";

    /// <summary>
    /// A unique 6 character code that can be used to reference the type. 
    /// The code should contain only single-byte (non-unicode) characters
    /// and although case-insensitive, the convention is to use uppercase
    /// e.g. "COFACR" represents the Cofoundry account recovery task.
    /// </summary>
    public string AuthorizedTaskTypeCode => Code;

    /// <summary>
    /// A unique name that succintly describes the task. Max 20 characters.
    /// </summary>
    public string Name => "Member Invite";
}
```

## Adding a new task

`AddAuthorizedTaskCommand` can be used to add a new task. The example below shows how you would do this in a Cofoundry [CQS command handler](/framework/data-access/cqs), but you could also do the same directly in a controller action or anywhere that supports DI.

```csharp
using Cofoundry.Core.Web;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

public class InviteMemberCommandHandler
    : ICommandHandler<InviteMemberCommand>
    , ISignedInPermissionCheckHandler
{
    private readonly IAdvancedContentRepository _contentRepository;
    private readonly IAuthorizedTaskTokenUrlHelper _authorizedTaskTokenUrlHelper;
    private readonly ISiteUrlResolver _siteUrlResolver;

    public InviteMemberCommandHandler(
        IAdvancedContentRepository contentRepository,
        IAuthorizedTaskTokenUrlHelper authorizedTaskTokenUrlHelper,
        ISiteUrlResolver siteUrlResolver
        )
    {
        _contentRepository = contentRepository;
        _authorizedTaskTokenUrlHelper = authorizedTaskTokenUrlHelper;
        _siteUrlResolver = siteUrlResolver;
    }

    public async Task ExecuteAsync(InviteMemberCommand command, IExecutionContext executionContext)
    {
        var user = executionContext.UserContext.ToRequiredSignedInContext();
        
        // Create a new task and token. Here we use task data to capture
        // the email address so it can be retrieved later on
        var token = await _contentRepository
            .AuthorizedTasks()
            .AddAsync(new()
            {
                AuthorizedTaskTypeCode = MemberInviteAuthorizedTaskType.Code,
                UserId = user.UserId,
                TaskData = command.EmailAddressToInvite
            });

        // Here we use IAuthorizedTaskTokenUrlHelper to insert the token into 
        // the url as a query parameter, but you can format this however you want
        var inviteUrl = _authorizedTaskTokenUrlHelper.MakeUrl("/members/register", token);

        // If you need to make the url absoute, you can use ISiteUrlResolver
        inviteUrl = _siteUrlResolver.MakeAbsolute(inviteUrl);

        // Send email 
        // (omitted)
    }
}
```

## Token Validation

`ValidateAuthorizedTaskTokenQuery` can be used to validate a token, returning an object indicating whether validation was successful and details of any error that was found. As an alternative to the CQS example above, the below example shows how we might write our validation logic directly into an ASP.NET controller:

```csharp
using Cofoundry.Domain;
using Cofoundry.Web;

[Route("members")]
public class MemberController : Controller
{
    private readonly IAdvancedContentRepository _contentRepository;
    private readonly IAuthorizedTaskTokenUrlHelper _authorizedTaskTokenUrlHelper;

    public MemberController(
        IAdvancedContentRepository contentRepository,
        IAuthorizedTaskTokenUrlHelper authorizedTaskTokenUrlHelper
        )
    {
        _contentRepository = contentRepository;
        _authorizedTaskTokenUrlHelper = authorizedTaskTokenUrlHelper;
    }

    [Route("register")]
    public async Task<IActionResult> Register()
    {
        // Init an empty view model to avoid returning null
        var viewModel = new RegisterNewUserViewModel();

        // First validate that the current user isn't signed in
        var isSignedIn = await _contentRepository
            .Users()
            .Current()
            .IsSignedIn()
            .ExecuteAsync();

        if (isSignedIn)
        {
            ModelState.AddModelError(string.Empty, "You cannot register because you are already signed in.");
            return View(viewModel);
        }

        // We use IAuthorizedTaskTokenUrlHelper to get the token from the query
        // however is optional and you can pass the token in any way you want
        var token = _authorizedTaskTokenUrlHelper.ParseTokenFromQuery(this.Request.Query);

        // Validating the token will return a result that describes any errors
        var result = await _contentRepository
            .AuthorizedTasks()
            .ValidateAsync(new()
            {
                AuthorizedTaskTypeCode = MemberInviteAuthorizedTaskType.Code,
                Token = token
            })
            .ExecuteAsync();

        // If not successful, add the error message to the ModelState.
        // Alternatively you can return the full error model to the view
        // or call result.ThrowIfNotSuccess() to throw an exception.
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error.Message);
        }

        return View(viewModel);
    }
}
```

In the above example we simply output the error message to the controller model state, but the authorized task error messages are fairly generic and you may want to customize the error messages or error handling logic to your task. The validation errors use string codes to represent the different types of errors that can occur, this makes it straightforward to adapt your logic either in .NET code or on the other side of an API boundary e.g. in JavaScript.

Errors that can occur are as follows:

- **"cf-authorized-tasks-token-validation-not-found"**: Invalid id and token combination. This can include situations where the id or token are not correctly formatted, or if the task cannot be located in the database.
- **"cf-authorized-tasks-token-validation-invalidated"**: The task has been invalidated by another action, such as another task performing the same action, or the action having been completed through a separate route.
- **"cf-authorized-tasks-token-validation-already-complete"**: The task exists but has already been completed.
- **"cf-authorized-tasks-token-validation-expired"**: The task exists but has expired.

In the example above, you could replace the standard error message with a call to this method:

```csharp
private void AddCustomErrorMessage(AuthorizedTaskTokenValidationResult result)
{
    if (result.IsSuccess)
    {
        return;
    }

    // Error codes are namespaced, so we remove the namespace here 
    // to cut down on repeated text
    const string ns = "cf-authorized-tasks-token-validation-";
    var codeWithoutNamespace = result.Error.ErrorCode?.Replace(ns, string.Empty);

    var message = codeWithoutNamespace switch
    {
        "not-found" => "Invite not found",
        "invalidated" => "Invite no longer valid",
        "already-complete" => "Invite already used",
        "expired" => "Invite expired, please request another",
        _ => result.Error.Message
    };

    ModelState.AddModelError(string.Empty, message);
}
```

Error codes can also be referenced via the static properties on `AuthorizedTaskValidationErrors` e.g. `AuthorizedTaskValidationErrors.TokenValidation.NotFound.Code`, however these aren't constant values and therefore cannot be used in `switch` statements.

## Completion

To mark a task as complete you can use `CompleteAuthorizedTaskCommand`. The command requires an `AuthorizedTaskId`, which is retrieved by validating a token. This ensures that a token is validated before a task is marked as complete.

This example shows how to validate the token and then complete the task in the context of an API controller:  

```csharp
using Cofoundry.Domain;
using Cofoundry.Web;

[Route("api/members")]
[ApiController]
public class MembersApiController : ControllerBase
{
    private readonly IAdvancedContentRepository _contentRepository;

    public MembersApiController(
        IAdvancedContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        var validationResult = await _contentRepository
            .WithModelState(this)
            .AuthorizedTasks()
            .ValidateAsync(new()
            {
                AuthorizedTaskTypeCode = MemberInviteAuthorizedTaskType.Code,
                Token = registerUserDto.Token
            })
            .ExecuteAsync();

        // WithModelState is used in the query to capture any errors and add them 
        // to ModelState, which can simplify our action
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Registering a user and marking the task complete should 
        // be run in a transaction
        using (var scope = _contentRepository.Transactions().CreateScope())
        {
            // Register user 
            // (omitted)

            // Mark task completed
            await _contentRepository
                .WithModelState(this)
                .AuthorizedTasks()
                .CompleteAsync(new()
                {
                    AuthorizedTaskId = validationResult.Data.AuthorizedTaskId
                });

            await scope.CompleteIfValidAsync(ModelState);
        }

        return Ok();
    }
}
```

## Invalidation

Sometimes you may need to invalidate a task or set of tasks because another action has caused them to no longer be valid. An example of this is with account recovery requests, where a user could generate multiple requests before resetting their password. In this case it is important to invalidate all other tasks when any one of them is completed. In addition, we also invalidate account recovery tasks when a user logs in or their password changes. We do this because these actions indicate that a user no longer needs an account recovery and old tokens should be invalidated as a security measure. 

You can invalidate all tasks for a user by executing `InvalidateAuthorizedTaskBatchCommand` with just a `UserId`, but often you'll also want to limit the operation to a specified task type:

```csharp
await _advancedContentRepository
    .AuthorizedTasks()
    .InvalidateBatchAsync(new()
    {
        UserId = userId,
        AuthorizedTaskTypeCodes = [MemberInviteAuthorizedTaskType.Code]
    })
    .ExecuteAsync();
```

## Other Settings

### Expiry

When adding a task, you can optionally set the time period that a task is valid. It is expected that this would be set in most cases, but it is not mandatory:

```csharp
var token = await _advancedContentRepository
    .AuthorizedTasks()
    .AddAsync(new()
    {
        AuthorizedTaskTypeCode = ExampleAuthorizedTaskType.Code,
        UserId = userId,
        ExpireAfter = TimeSpan.FromDays(1)
    });
```

### Rate Limiting

When adding a task, you can optionally set a rate limit as a way to mitigate abuse:

```csharp
var token = await _advancedContentRepository
    .AuthorizedTasks()
    .AddAsync(new()
    {
        AuthorizedTaskTypeCode = ExampleAuthorizedTaskType.Code,
        UserId = userId,
        RateLimit = new()
        {
            Quantity = 3,
            Window = TimeSpan.FromHours(6)
        }
    });
```

## Cleanup

If [background tasks](/framework/background-tasks) are enabled, a background task will periodically run to delete completed, invalid or expired authorized tasks from the database after a period of time. By default the retention period is 30 days, but this can be change in [config settings](/reference/common-config-settings#AuthorizedTaskCleanupSettings).