namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.DTOs;

public class TraderUpdateDTO
{
    public string? MessageId { get; set; }
    public string? MessageType { get; set; }
    public TraderPartyUpdateDTO? TraderPartyUpdate { get; set; }

}
