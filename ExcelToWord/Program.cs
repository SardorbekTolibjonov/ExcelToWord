using ExcelToWord.Extensions;

using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.ConfigureDefaults("ExcelToWord");

var botToken = builder.Configuration["BotConfiguration:Token"];
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));
builder.Services.AddHostedService<TelegramBotService>();

var app = builder.Build();

await app.ConfigureDefaults();

await app.RunAsync();
