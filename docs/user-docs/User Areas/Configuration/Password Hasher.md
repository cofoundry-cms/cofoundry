By default, Cofoundry defers password hashing to the ASP.NET Identity password hasher. Doing so ensures that Cofoundry is aligned with the password hashing security standards supported by the ASP.NET team, however, their implementation is restricted by [what is available in the CLR](https://github.com/dotnet/aspnetcore/issues/37032#issuecomment-984030449) and at the time of writing is configured with a relatively low iteration rate.

Scott Brady [outlines some of these issues in this blog post ](https://www.scottbrady91.com/aspnet-identity/improving-the-aspnet-core-identity-password-hasher), and has created some packages that use more secure options. Selecting, trusting and configuring an algorithm requires some thought though, and selecting a more secure configuration will affect the time it takes to authenticate as well as the computational load on your server, so in many cases it's more appropriate just to trust the ASP.NET implementation. However if you understand the implications and do want to swap out the password hasher, then read on.

## Configuring the default password hasher

Because the default hasher uses the ASP.NET Core Identity `IPasswordHasher`, it can be configured as normal outside of Cofoundry via `PasswordHasherOptions` in your `Program.cs` file:

```csharp
builder.Services
    .AddMvc()
    .AddCofoundry(builder.Configuration);

builder.Services.Configure<PasswordHasherOptions>(options => options.IterationCount = 512000);
```

## Overriding the default password hasher

To override the default `IPasswordHasher` implementation, simply replace the registered `IPasswordHasher<PasswordHasherUser>` with your own:

```csharp
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Cryptography;
using Microsoft.AspNetCore.Identity;

public class ExamplePasswordHasherRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        var overrideOptions = RegistrationOptions.Override();
        container.Register<IPasswordHasher<PasswordHasherUser>, ExamplePasswordHasher<PasswordHasherUser>>(overrideOptions);
    }
}
```

## Configuring the authentication execution duration

To mitigate timing-based enumeration attacks on authentication attempts Cofoundry uses a "minimum random duration" technique that forces all authentication attempts to execute with a random duration between set bounds. This ensures that it's not possible to use the timing of a request to work out whether a username exists or not.

The minimum value of the duration should be set to a value beyond the expected duration of an authentication attempt. By default this is set to 1.5 seconds, but if you choose a hashing algorithm that increases the authentication time significantly you'll need to account for this.

The authentication duration can be configured using these configuration settings:

- **Cofoundry:Users:Authentication:ExecutionDuration:MinInMilliseconds** The inclusive lower bound of the randomized credential authorization execution duration, measured in milliseconds (1000ms = 1s). Defaults to 1.5 second.
- **Cofoundry:Users:Authentication:ExecutionDuration:MaxInMilliseconds** The inclusive upper bound of the randomized credential authorization execution duration, measured in milliseconds (2000ms = 2s). Defaults to 2 seconds.

