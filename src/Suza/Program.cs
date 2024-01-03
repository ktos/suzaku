using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Suza.Services;
using Suzaku.Bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenAiNg;
using Suzaku.Shared;
using Suzaku.Bot.Models;

var config = new Dictionary<string, string?>() { { "Bot:Name", "Suza" } };

IConfiguration configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddInMemoryCollection(config)
    .AddUserSecrets<Program>()
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddLogging(builder => builder.AddConsole());

        services.Configure<MqttConfiguration>(configuration.GetSection("Mqtt"));
        services.Configure<BotConfiguration>(configuration.GetSection("Bot"));
        services.AddSingleton<PromptProvider>();
        services.AddSingleton(
            new OpenAiApi(configuration.GetSection("OpenAI")["ApiKey"])
            {
                ApiUrlFormat = configuration.GetSection("OpenAI")["Url"]
            }
        );
        services.AddSingleton<IMessageResponder, OpenAiMessageResponder>();
        services.AddSingleton<MqttService>();
    })
    .Build();

var mqtt = host.Services.GetRequiredService<MqttService>();
var log = host.Services.GetRequiredService<ILogger<Program>>();

log.LogInformation("Initializing MQTT connection");
await mqtt.InitializeAsync();

log.LogInformation("Working until shutdown...");

host.WaitForShutdown();
