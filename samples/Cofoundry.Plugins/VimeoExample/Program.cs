﻿using Cofoundry.Web;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseLocalConfigFile();

builder.Services
    .AddControllersWithViews()
    .AddCofoundry(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCofoundry();

app.Run();
