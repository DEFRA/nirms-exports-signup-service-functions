CREATE PROCEDURE [dbo].[sp_UpdateTradeParty] 
	@OrgId uniqueidentifier,
	@Status int
AS

BEGIN
	DECLARE @partyId uniqueidentifier;
	DECLARE @ApprovalStatus int;

	SET @ApprovalStatus = @Status;
	SET @partyId = (SELECT Id FROM dbo.TradeParties WHERE OrgId = @OrgId AND ApprovalStatus != 2);

	UPDATE dbo.TradeParties
	SET ApprovalStatus = @ApprovalStatus
	WHERE Id = @partyId;
	
	UPDATE dbo.LogisticsLocation
    SET ApprovalStatus = @ApprovalStatus
    WHERE TradePartyId = @partyId;

END
GO
