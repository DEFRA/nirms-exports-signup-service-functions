using Defra.Trade.Common.Config;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Infrastructure;

public sealed class ServiceBusQueuesSettings : ServiceBusSettings
{
    public string? QueueNameEhcoRemosEnrichment { get; set; }
    public string? QueueNameEhcoRemosCreate { get; set; }
}
