/****** Object:  StoredProcedure [dbo].[sp_UpdateTradeParty]    Script Date: 12/04/2024 15:34:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_UpdateTradeParty] 
	@OrgId uniqueidentifier,
	@Status int
AS
 
BEGIN
	DECLARE @partyId uniqueidentifier;
	DECLARE @ApprovalStatus int;
 
	SET @ApprovalStatus = @Status;
	SET @partyId = (SELECT Id FROM dbo.TradeParties WHERE OrgId = @OrgId AND ApprovalStatus != 2 AND RemosBusinessSchemeNumber IS NOT NULL);
 
	UPDATE dbo.TradeParties
	SET ApprovalStatus = @ApprovalStatus
	WHERE Id = @partyId;
END