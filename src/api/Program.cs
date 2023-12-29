global using Microsoft.EntityFrameworkCore;
global using RFIDify.Database;
using RFIDify;

var builder = WebApplication.CreateBuilder(args);
builder.AddServices();
var app = builder.Build();
await app.Configure();

app.Logger.LogInformation("Starting application. Environment: {Environment}", builder.Environment.EnvironmentName);
app.Run();