using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.DTOs;

public class EstablishmentStatusUpdateDTO
{
    public Guid InspectionLocationId { get; set; }
    public string? RemosId { get; set; }
    public string? ApprovalStatus { get; set; }
    public string? ActiveStatus { get; set; }
}
