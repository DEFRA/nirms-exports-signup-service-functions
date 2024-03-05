using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Models;

[ExcludeFromCodeCoverage]
public class AppConfigurationSettings
{
    public string? SqlConnection { get; set; }
}
