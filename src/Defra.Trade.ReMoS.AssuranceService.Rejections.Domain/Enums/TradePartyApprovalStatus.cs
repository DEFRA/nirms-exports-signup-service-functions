using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.Rejections.Domain.Enums;

public enum TradePartyApprovalStatus
{
    [Description("NOT SIGNED-UP")]
    NotSignedUp,

    [Description("APPROVED FOR NIRMS")]
    Approved,

    [Description("SIGN-UP REJECTED")]
    Rejected,

    [Description("SIGN-UP STARTED")]
    SignupStarted,

    [Description("PENDING APPROVAL")]
    PendingApproval,

    [Description("SUSPENDED")]
    Suspended
}
