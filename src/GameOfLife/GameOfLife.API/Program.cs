using Common.Web.Extensions;
using GameOfLife.API.Extensions;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddCommonApi();
builder.AddConsoleLogging();
builder.UseAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.RegisterServices(builder.Configuration);

WebApplication app = builder.Build();
app.UseCommonApi();

app.Run();
