using Cofoundry.Web;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseLocalConfigFile();

builder.Services
    .AddMvc()
    .AddCofoundry(builder.Configuration);

var app = builder.Build();

app.UseCofoundry();

app.Run();
