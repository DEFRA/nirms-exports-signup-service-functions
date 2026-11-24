using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Config;
using Defra.Trade.Common.Functions.Isolated.EventStore;
using Defra.Trade.Common.Functions.Isolated.Interfaces;
using Defra.Trade.Common.Functions.Isolated.Validation;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Infrastructure;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Infrastructure;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICustomValidatorFactory, CustomValidatorFactory>();

        services.AddEventStoreConfiguration();

        services.AddTransient<ISchemaValidator, SchemaValidator>();

        services.AddSingleton<IMessageCollector, EventStoreCollector>();

        services.AddOptions<ServiceBusQueuesSettings>().Bind(configuration.GetSection(ServiceBusSettings.OptionsName));

        var gcConfig = configuration.GetSection(RemosSignUpSubscriberSettings.RemosSignUpSubscriberSettingsName);
        services.AddOptions<RemosSignUpSubscriberSettings>().Bind(gcConfig);

        services.Configure<ServiceBusSettings>(configuration.GetSection(ServiceBusSettings.OptionsName));

        services.AddSingleton(_ =>
        {
#if DEBUG
            string connectionString = configuration.GetValue<string>(RemosSignUpSubscriberSettings.ConnectionStringConfigurationKey);
            return new ServiceBusClient(connectionString);
#else
            string connectionString = configuration.GetValue<string>($"{RemosSignUpSubscriberSettings.ConnectionStringConfigurationKey}FQN");
            return new ServiceBusClient(connectionString, new Azure.Identity.DefaultAzureCredential());
#endif
        });

        return services;
    }
}