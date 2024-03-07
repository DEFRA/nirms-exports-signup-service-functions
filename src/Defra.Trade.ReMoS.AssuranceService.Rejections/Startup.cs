using Defra.Trade.Common.AppConfig;
using Defra.Trade.ReMoS.AssuranceService.API.Data.Persistence.Context;
using Defra.Trade.ReMoS.AssuranceService.Rejections;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Defra.Trade.Common.Logging.Extensions;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Infrastructure;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Infrastructure;
using Defra.Trade.Common.Config;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Defra.Trade.ReMoS.AssuranceService.Rejections;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = builder.GetContext().Configuration;

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.GetContext().Configuration.GetConnectionString("sql_db")));

        builder.Services
         .AddTradeAppConfiguration(configuration)
         .AddServiceRegistrations(configuration)
         .AddApplication()
         .AddFunctionLogging("RejectTrader");

        builder.ConfigureMapper();
    }


    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        builder.ConfigurationBuilder
           .ConfigureTradeAppConfiguration(opt =>
           {
               opt.UseKeyVaultSecrets = false;
               opt.RefreshKeys.Add($"{RemosSignUpSubscriberSettings.RemosSignUpSubscriberSettingsName}:{RemosSignUpSubscriberSettings.AppConfigSentinelName}");
               opt.Select<ConfigurationServerSettings>(ConfigurationServerSettings.OptionsName);
               opt.Select<ServiceBusSettings>(ServiceBusSettings.OptionsName);
               opt.Select<RemosSignUpSubscriberSettings>(RemosSignUpSubscriberSettings.RemosSignUpSubscriberSettingsName);
           });
    }
}
