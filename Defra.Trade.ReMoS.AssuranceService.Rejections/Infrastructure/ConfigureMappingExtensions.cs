using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Infrastructure;

public static class ConfigureMappingExtensions
{
    public static void ConfigureMapper(this IFunctionsHostBuilder hostBuilder)
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName is string fullName && fullName.Contains("Defra"))
            .OrderBy(a => a.FullName)
            .ToList();

        hostBuilder.Services.AddAutoMapper(assembly);
    }
}