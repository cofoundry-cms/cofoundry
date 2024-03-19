﻿using Microsoft.AspNetCore.Builder;

namespace Cofoundry.Web;

/// <summary>
/// Adds the asp.net auth middleware into the pipeline.
/// </summary>
public class AuthStartupConfigurationTask
    : IStartupConfigurationTask
    , IRunAfterStartupConfigurationTask
{
    public int Ordering
    {
        get { return (int)StartupTaskOrdering.Early; }
    }

    public IReadOnlyCollection<Type> RunAfter => [typeof(UseRoutingStartupConfigurationTask)];

    public void Configure(IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
