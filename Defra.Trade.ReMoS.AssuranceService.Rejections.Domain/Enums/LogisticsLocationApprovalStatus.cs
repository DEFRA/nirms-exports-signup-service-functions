using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Enums;
public enum LogisticsLocationApprovalStatus
{
    None,
    Approved,
    Rejected,
    Draft,
    PendingApproval,
    Suspended,
    Removed
}
