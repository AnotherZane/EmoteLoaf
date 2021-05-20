using System;
using System.Linq;
using System.Net.Http;
using Disqord;
using Disqord.Bot.Hosting;
using Disqord.Extensions.Interactivity;
using Disqord.Gateway;
using EmoteLoaf.Disqord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace EmoteLoaf
{
    public sealed class Program
    {
        private const string ConfigPath = "./config.json";
        
        public static void Main(string[] args)
        {
            try
            {
                var host = new HostBuilder()
                    .ConfigureLogging(x =>
                    {
                        var logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .WriteTo.Console(
                                outputTemplate:
                                "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                                theme: AnsiConsoleTheme.Code)
                            .WriteTo.File($"logs/log.txt", rollingInterval: RollingInterval.Day,
                                restrictedToMinimumLevel: LogEventLevel.Information, retainedFileCountLimit: null, 
                                fileSizeLimitBytes: null, buffered: true)
                            .CreateLogger();

                        x.AddSerilog(logger, true);

                        x.Services.Remove(x.Services.First(x => x.ServiceType == typeof(ILogger<>)));
                        x.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
                    })
                    .ConfigureHostConfiguration(configuration => configuration.AddJsonFile(ConfigPath))
                    .ConfigureServices((context, services) =>
                    {
                        services.AddInteractivity();
                        services.AddSingleton<Random>();
                        services.AddSingleton<HttpClient>();
                    })
                    .ConfigureDiscordBot<EmoteLoafBot>((context, bot) =>
                    {
                        bot.Token = context.Configuration["discord:token"];
                        // no implicit conversion cause they're considered longs
                        bot.OwnerIds = new[] {new Snowflake(406533583587770369), new Snowflake(608143610415939638)};
                        bot.Prefixes = new[] {"el/", "em/", "ed/"};
                        bot.Intents = GatewayIntents.All;
                    })
                    .Build();

                using (host)
                {
                    host.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }
    }
}
