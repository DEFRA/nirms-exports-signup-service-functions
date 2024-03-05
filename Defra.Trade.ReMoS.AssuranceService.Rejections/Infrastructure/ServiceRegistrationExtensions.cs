using Defra.Trade.Common.Config;
using Defra.Trade.Common.Functions.EventStore;
using Defra.Trade.Common.Functions.Interfaces;
using Defra.Trade.Common.Functions.Validation;
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

        return services;
    }
}