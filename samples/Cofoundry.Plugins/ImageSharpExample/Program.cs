using Cofoundry.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorPages()
    .AddCofoundry(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCofoundry();

app.Run();
