﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Web;

/// <summary>
/// Configures the error pages returned by the application for both exceptions and
/// error status codes such as 404s. This is done using the built-in the ASP.NET 
/// error handling and status code pages middleware. 
/// </summary>
public class ErrorHandlingMiddlewareConfigurationTask : IStartupConfigurationTask
{
    private readonly DebugSettings _debugSettings;

    public ErrorHandlingMiddlewareConfigurationTask(
        DebugSettings debugSettings
        )
    {
        _debugSettings = debugSettings;
    }

    public int Ordering => (int)StartupTaskOrdering.First;

    public void Configure(IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

        if (_debugSettings.CanShowDeveloperExceptionPage(env))
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler(CofoundryErrorController.ExceptionHandlerPath);
        }

        app.UseStatusCodePagesWithReExecute(CofoundryErrorController.StatusCodePagesRoute);
    }
}
