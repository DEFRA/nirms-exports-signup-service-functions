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
    public class EstablishmentFunction(ApplicationDbContext dbContext, ILogger<EstablishmentFunction> logger)
    {
        [Function("EstablishmentFunction")]
        public async Task Run([ServiceBusTrigger(RemosSignUpSubscriberSettings.EstablishmentQueueName, Connection = RemosSignUpSubscriberSettings.ConnectionStringConfigurationKey, IsSessionsEnabled = false, AutoCompleteMessages = false)] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
        {
            logger.LogInformation($"C# ServiceBus queue trigger function processed message: {message.Body}");
            if (message.Body != null && message.Body.ToString() != "null")
            {
                try
                {
                    int StatusInt;

                    var result = JsonConvert.DeserializeObject<EstablishmentUpdateDTO>(message.Body.ToString());

                    if (result.EstablishmentStatusUpdate.ActiveStatus == "Inactive")
                    {
                        StatusInt = (int)LogisticsLocationApprovalStatus.Removed;
                    }
                    else
                    {
                        StatusInt = result.EstablishmentStatusUpdate.ApprovalStatus switch
                        {
                            "Approved" => (int)LogisticsLocationApprovalStatus.Approved,
                            "Suspended" => (int)LogisticsLocationApprovalStatus.Suspended,
                            "Rejected" => (int)LogisticsLocationApprovalStatus.Rejected,
                            _ => (int)LogisticsLocationApprovalStatus.NoUpdate,
                        };
                    }
                    dbContext.Database.ExecuteSqlRaw(@"EXECUTE sp_UpdateEstablishmentStatus {0}, {1}, {2}", 
                        StatusInt.ToString(), result.EstablishmentStatusUpdate.InspectionLocationId.ToString(), result.EstablishmentStatusUpdate.RemosId.ToString());
                }
                catch (JsonException ex)
                {
                        await messageActions.DeadLetterMessageAsync(message, deadLetterReason: ex.Message, deadLetterErrorDescription: ex.Message);
                        logger.LogError($"Exception hit processing Queue item: {ex}");
                        return;
                }

                catch (Exception e)
                {
                    logger.LogError($"Error executing command, exception: {e}");
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