var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseLocalConfigFile();
builder.Services
    .AddControllersWithViews()
    .AddCofoundry(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseCofoundry();

app.Run();
