using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Suza.Services;
using Suzaku.Bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenAiNg;
using Suzaku.Chat;

IConfiguration configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddLogging(builder => builder.AddConsole());

        services.Configure<MqttConfiguration>(configuration.GetSection("Mqtt"));
        services.AddSingleton<PromptProvider>();
        services.AddSingleton(
            new OpenAiApi(configuration.GetSection("OpenAI")["ApiKey"])
            {
                ApiUrlFormat = configuration.GetSection("OpenAI")["Url"]
            }
        );
        services.AddSingleton<IMessageResponder, BasicMessageResponder>();
        services.AddSingleton<MqttService>();
    })
    .Build();

var mqtt = host.Services.GetRequiredService<MqttService>();
var log = host.Services.GetRequiredService<ILogger<Program>>();

log.LogInformation("Initializing MQTT connection");
await mqtt.InitializeAsync();

log.LogInformation("Working until shutdown...");

host.WaitForShutdown();
