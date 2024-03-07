using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.DTOs;

public class TraderPartyUpdateDTO
{
    public string? Status { get; set; }
    public Guid OrganisationId { get; set; }
}
