using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Config;
using Defra.Trade.ReMoS.AssuranceService.API.Data.Persistence.Context;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Infrastructure;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Models;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((_, configurationBuilder) =>
    {
        configurationBuilder.ConfigureTradeAppConfiguration(options =>
        {
            options.UseKeyVaultSecrets = false;
            options.RefreshKeys.Add($"{RemosSignUpSubscriberSettings.RemosSignUpSubscriberSettingsName}:{RemosSignUpSubscriberSettings.AppConfigSentinelName}");
            options.Select<ConfigurationServerSettings>(ConfigurationServerSettings.OptionsName);
            options.Select<ServiceBusSettings>(ServiceBusSettings.OptionsName);
            options.Select<RemosSignUpSubscriberSettings>(RemosSignUpSubscriberSettings.RemosSignUpSubscriberSettingsName);
        });
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("sql_db")));

        services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights()
            .AddTradeAppConfiguration(configuration)
            .AddServiceRegistrations(configuration)
            .AddApplication();

        services.ConfigureMapper();

        // The Application Insights SDK registers a default logging filter rule that
        // restricts the ApplicationInsightsLoggerProvider to Warning and above, which
        // overrides host.json's "logging:logLevel" configuration. Remove it so that
        // host.json settings are honoured and Information-level application logs are sent.
        services.Configure<LoggerFilterOptions>(options =>
        {
            var defaultRule = options.Rules.FirstOrDefault(rule =>
                rule.ProviderName == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");

            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
            }
        });
    })
    .Build();

host.Run();