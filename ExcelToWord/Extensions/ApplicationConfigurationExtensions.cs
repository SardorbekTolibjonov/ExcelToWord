using System.Reflection;
using System.Text.Json.Serialization;
using BRB.Core.EF.Extensions;
using BRB.Core.Web.Fallback;
using BRB.Core.Web.Middlewares;
using ExcelToWord.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerUI;
using Telegram.Bot;

namespace ExcelToWord.Extensions;

public static class ApplicationConfigurationExtensions
{
    public static WebApplicationBuilder ConfigureDefaults(this WebApplicationBuilder builder, string appName)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        builder
            .ConfigureHostConfigurations(appName)
            .ConfigureLogger()
            .ConfigureSwagger(appName)
            .ConfigureControllers()
            .ConfigureGlobalExceptionHandler()
            .AddServices()
            .AddTelegramClient();

        return builder;
    }
    
    public static async  Task<WebApplication> ConfigureDefaults(this WebApplication app, string? baseUrl = null)
    {
        if (!app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.ConfigObject.AdditionalItems.Add("persistAuthorization", true);
                options.DocExpansion(DocExpansion.None);
            });
        }

        app.UseCors();

        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        // app.UseHealthChecks("/healthy");
        app.UseAuthorization();
        app.UseCustom404Page("");
        app.MapControllers();

        return app;
    }
    private static WebApplicationBuilder ConfigureHostConfigurations(this WebApplicationBuilder builder,
        string appName)
    {
        _ = builder.Configuration.AddJsonFile(
            Path.Join(AppContext.BaseDirectory,
                $"appsettings.{builder.Environment.EnvironmentName}.json"),
            optional: false);
        _ = builder.Configuration.AddJsonFile(
            Path.Join(AppContext.BaseDirectory,
                $"appsettings.json"),
            optional: false);
        builder.Configuration.AddEnvironmentVariables();

        return builder;
    }

    private static WebApplicationBuilder ConfigureLogger(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(new LoggingLevelSwitch(LogEventLevel.Information))
            .Enrich.FromLogContext()
            .WriteTo
            .Console(LogEventLevel.Debug)
            .CreateLogger();

        Log.Information("Project started at {0} with PID: {1}",
            DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"),
            Environment.ProcessId);

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();

        return builder;
    }
    private static WebApplicationBuilder ConfigureSwagger(this WebApplicationBuilder builder, string appName)
    {
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo()
                {
                    Title = appName,
                    Version = "v1"
                });

            var xmlFile = $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml";
            var xmlPath = Path.Join(AppContext.BaseDirectory, xmlFile);

            options.CustomSchemaIds(type => type.FullName);
            // options.IncludeXmlComments(xmlPath);
            /*options.OperationFilter<MlfHeaderFilter>();
            options.OperationFilter<PermissionFilter>();

            var securityScheme = new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Cookie and Header based Authentication",
                Name = "Jwt",
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            };

            options.AddSecurityDefinition("Bearer", securityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    securityScheme, new List<string>()
                    {
                        "Bearer"
                    }
                }
            });*/
        });

        builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
        builder.Services.AddCookiePolicy(options => { options.Secure = CookieSecurePolicy.Always; });

        return builder;
    }
    private static WebApplicationBuilder ConfigureControllers(this WebApplicationBuilder builder)
    {
        IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        builder.Services.AddSingleton(httpContextAccessor);

        builder.Services.AddControllers(options =>
        {
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            // options.Filters.Add<ModelValidationFilter>();
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            // options.JsonSerializerOptions.Converters.Add(new MultiLanguageFieldConverter(httpContextAccessor));
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        return builder;
    }
    
    private static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        // builder.Services.ConfigureServicesFromTypeAssembly<AuthService>();
        builder.Services.AddMemoryCache();
        builder.Services.AddHttpContextAccessor();
        builder.Services.ConfigureServicesFromTypeAssembly<ReportService>();
        
        builder.Services.AddHttpClient("OfficeTools",
            client => { client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("OfficeToolsUrl")!); });

        return builder;
    }
    
    private static WebApplicationBuilder ConfigureGlobalExceptionHandler(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<GlobalExceptionHandlerMiddleware>();
        return builder;
    }

    private static WebApplicationBuilder AddTelegramClient(this WebApplicationBuilder builder)
    {
        var botToken = builder.Configuration["BotConfiguration:Token"];
        builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));
        builder.Services.AddHostedService<TelegramBotService>();
        return builder;
    }
    
}