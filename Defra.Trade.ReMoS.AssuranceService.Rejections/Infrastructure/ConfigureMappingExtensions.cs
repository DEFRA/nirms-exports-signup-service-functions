using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Infrastructure;

public static class ConfigureMappingExtensions
{
    public static IServiceCollection ConfigureMapper(this IServiceCollection services)
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName is string fullName && fullName.Contains("Defra"))
            .OrderBy(a => a.FullName)
            .ToList();

        services.AddAutoMapper(_ => { }, assembly.ToArray());
        return services;
    }
}