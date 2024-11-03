var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllersWithViews()
    .AddCofoundry(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCofoundry();

app.Run();
