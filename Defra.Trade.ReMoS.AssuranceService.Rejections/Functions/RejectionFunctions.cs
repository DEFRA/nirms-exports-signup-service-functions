using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Defra.Trade.ReMoS.AssuranceService.API.Data.Persistence.Context;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.DTOs;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Enums;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Functions
{
    public class RejectionFunctions(ApplicationDbContext dbContext, ILogger<RejectionFunctions> logger)
    {
        [Function("RejectTrader")]
        public async Task Run(
            [ServiceBusTrigger(RemosSignUpSubscriberSettings.DefaultQueueName, 
                               Connection = RemosSignUpSubscriberSettings.ConnectionStringConfigurationKey, 
                               IsSessionsEnabled = false, 
                               AutoCompleteMessages = false)] ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            logger.LogInformation($"C# ServiceBus queue trigger function processed message: {message.Body}");
            if (message.Body != null && message.Body.ToString() != "null")
            {
                try
                {
                    TraderUpdateDTO result = JsonConvert.DeserializeObject<TraderUpdateDTO>(message.Body.ToString());
                    var StatusInt = result.TraderPartyUpdate.Status switch
                    {
                        "Approved" => (int)TradePartyApprovalStatus.Approved,
                        "Rejected" => (int)TradePartyApprovalStatus.Rejected,
                        "Suspended" => (int)TradePartyApprovalStatus.Suspended,
                        _ => (int)TradePartyApprovalStatus.PendingApproval,
                    };
                    dbContext.Database.ExecuteSqlRaw(@"EXECUTE sp_UpdateTradeParty {0}, {1}", result.TraderPartyUpdate.OrganisationId.ToString(), StatusInt.ToString());
                }

                catch (JsonException ex)
                {
                    await messageActions.DeadLetterMessageAsync(message, deadLetterReason: ex.Message, deadLetterErrorDescription: ex.Message);
                    logger.LogError($"Exception hit processing Queue item: {ex}");
                    return;
                }

                catch (Exception e)
                {
                    logger.LogError($"Error executing sp_UpdateTradeParty, exception: {e}");
                    throw;
                }
                await messageActions.CompleteMessageAsync(message);
            }

            else
            {
                await messageActions.CompleteMessageAsync(message);
                logger.LogError("Queue item is null, no action performed");
            }
        }

    }

}
