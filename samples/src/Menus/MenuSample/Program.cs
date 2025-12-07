var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMvc()
    .AddCofoundry(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCofoundry();

app.Run();
