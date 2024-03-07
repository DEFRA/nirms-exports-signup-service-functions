namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.DTOs;

public class EstablishmentUpdateDTO
{
    public string? MessageId { get; set; }
    public string? MessageType { get; set; }
    public EstablishmentStatusUpdateDTO? EstablishmentStatusUpdate { get; set; }
}
