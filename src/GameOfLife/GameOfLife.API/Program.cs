using Common.Web.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddCommonApi();
builder.AddConsoleLogging();

WebApplication app = builder.Build();
app.UseCommonApi();

app.Run();
