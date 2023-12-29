global using Microsoft.EntityFrameworkCore;
global using RFIDify.Database;
global using Microsoft.AspNetCore.Http.HttpResults;
global using RFIDify.RFID.DataTypes;
using RFIDify;

var builder = WebApplication.CreateBuilder(args);
builder.AddServices();
var app = builder.Build();
await app.Configure();

app.Logger.LogInformation("Starting application. Environment: {Environment}", builder.Environment.EnvironmentName);
app.Run();