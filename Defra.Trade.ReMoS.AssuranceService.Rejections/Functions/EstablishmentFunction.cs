using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Defra.Trade.ReMoS.AssuranceService.API.Data.Persistence.Context;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.DTOs;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Enums;
using Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;


using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Functions
{
    public class EstablishmentFunction
    {
        private readonly ApplicationDbContext _dbContext;

        public EstablishmentFunction(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [ServiceBusAccount(RemosSignUpSubscriberSettings.ConnectionStringConfigurationKey)]
        [FunctionName("EstablishmentFunction")]
        public async Task Run([ServiceBusTrigger(queueName: RemosSignUpSubscriberSettings.EstablishmentQueueName, IsSessionsEnabled = false)] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        ExecutionContext executionContext, Microsoft.Extensions.Logging.ILogger logger)
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
                    _dbContext.Database.ExecuteSqlRaw(@"EXECUTE sp_UpdateEstablishmentStatus {0}, {1}, {2}", 
                        StatusInt.ToString(), result.EstablishmentStatusUpdate.InspectionLocationId.ToString(), result.EstablishmentStatusUpdate.RemosId.ToString());
                }
                catch (JsonException ex)
                {
                    await messageActions.DeadLetterMessageAsync(message, ex.Message);
                    logger.LogError($"Exception hit processing Queue item: {ex}");
                }

                catch (Exception e)
                {
                    logger.LogError($"Error executing command, exception: {e}");
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