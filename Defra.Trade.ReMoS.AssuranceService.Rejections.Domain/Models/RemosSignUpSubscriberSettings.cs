namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Models;

public sealed class RemosSignUpSubscriberSettings
{

    public const string RemosSignUpSubscriberSettingsName = "EhcoGcSubscriber";
#if DEBUG

    // In 'Debug' (locally) use connection string
    public const string ConnectionStringConfigurationKey = "ServiceBus:ConnectionString";

#else
    // Assumes that this is 'Release' and uses Managed Identity rather than connection string
    // ie it will actually bind to ServiceBus:FullyQualifiedNamespace !
    public const string ConnectionStringConfigurationKey = "ServiceBus";
#endif

    public const string DefaultQueueName = "defra.trade.sus.rejections";
    public const string EstablishmentQueueName = "defra.trade.sus.establishment";
    public const string PublisherId = "REMOS";
    public const string TradeEventInfo = Common.Functions.Constants.QueueName.DefaultEventsInfoQueueName;

    public const string AppConfigSentinelName = "Sentinel";
    public string RemosSignUpCreatedQueue { get; set; } = DefaultQueueName;
}
