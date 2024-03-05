using Microsoft.Extensions.DependencyInjection;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
