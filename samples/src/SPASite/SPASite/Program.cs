using Microsoft.AspNetCore.SpaServices;
using VueCliMiddleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/dist";
});

builder.Services
    .AddMvc()
    .AddCofoundry(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSpaStaticFiles();
app.UseCofoundry();

// Un-comment this to run the vue cli automatically when debugging
// You'll need to install the vue cli, see https://cli.vuejs.org/guide/installation.html 
app.MapToVueCliProxy(
    "{*path}",
    new SpaOptions { SourcePath = "ClientApp" },
    npmScript: null, //(System.Diagnostics.Debugger.IsAttached) ? "serve" : null,
    regex: "Compiled successfully",
    forceKill: true
    );

app.Run();
