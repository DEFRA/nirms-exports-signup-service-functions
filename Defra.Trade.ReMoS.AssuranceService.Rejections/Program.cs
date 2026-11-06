using Defra.Trade.Common.AppConfig;
using Defra.Trade.Common.Config;
using Defra.Trade.Common.Logging.Extensions;
using Defra.Trade.ReMoS.AssuranceService.API.Data.Persistence.Context;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Infrastructure;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Models;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
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
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("sql_db")));

        services
            .AddTradeAppConfiguration(configuration)
            .AddServiceRegistrations(configuration)
            .AddApplication()
            .AddFunctionLogging("RejectTrader");

        services.ConfigureMapper();
    })
    .Build();

host.Run();