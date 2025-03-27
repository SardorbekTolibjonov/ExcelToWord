using ExcelToWord.Extensions;

using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.ConfigureDefaults("ExcelToWord");

var app = builder.Build();

await app.ConfigureDefaults();

await app.RunAsync();
