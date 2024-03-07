using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Constants;

public static class ApplicationConstants
{
    public static class ServiceBus
    {
        public const string ConnectionString = "ServiceBusConnectionString";
        public const string QueueName = "defra.trade.sus.rejections";
    }
}
