using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions.Extensions;
using Defra.Trade.ReMoS.AssuranceService.API.Data.Persistence.Context;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.DTOs;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Enums;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Functions
{
    public class RejectionFunctions
    {
        private readonly ApplicationDbContext _dbContext;

        public RejectionFunctions(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [ServiceBusAccount(RemosSignUpSubscriberSettings.ConnectionStringConfigurationKey)]
        [FunctionName("RejectTrader")]
        public async Task Run([ServiceBusTrigger(queueName: RemosSignUpSubscriberSettings.DefaultQueueName, IsSessionsEnabled = false)] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        ExecutionContext executionContext, Microsoft.Extensions.Logging.ILogger logger)
        {
            logger.LogInformation($"C# ServiceBus queue trigger function processed message: {message.Body}");
            if (message.Body != null && message.Body.ToString() != "null")
            {
                TraderUpdateDTO result = new();

                try
                {
                    result = JsonConvert.DeserializeObject<TraderUpdateDTO>(message.Body.ToString());

                    int StatusInt;

                    switch (result.TraderPartyUpdate.Status)
                    {
                        case "Approved":
                            StatusInt = (int)TradePartyApprovalStatus.Approved;
                            break;

                        case "Rejected":
                            StatusInt = (int)TradePartyApprovalStatus.Rejected;
                            break;

                        case "Suspended":
                            StatusInt = (int)TradePartyApprovalStatus.Suspended;
                            break;

                        default:
                            StatusInt = (int)TradePartyApprovalStatus.PendingApproval;
                            break;

                    }
                    _dbContext.Database.ExecuteSqlRaw(@"EXECUTE sp_UpdateTradeParty {0}, {1}", result.TraderPartyUpdate.OrganisationId.ToString(), StatusInt.ToString());
                }

                catch (Newtonsoft.Json.JsonException ex)
                {
                    await messageActions.DeadLetterMessageAsync(message, ex.Message);
                    logger.LogError($"Exception hit processing Queue item: {ex}");
                }

                catch (Exception e)
                {
                    logger.LogError($"Error executing sp_UpdateTradeParty, exception: {e.ToString()}");
                    throw;
                }
            }

            else
            {
                await messageActions.CompleteMessageAsync(message);
                logger.LogError($"Queue item is null, no action performed");
            }
        }

    }

}
