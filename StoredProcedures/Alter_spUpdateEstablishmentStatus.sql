﻿/****** Object:  StoredProcedure [dbo].[sp_UpdateEstablishmentStatus]    Script Date: 12/04/2024 14:23:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdateEstablishmentStatus] 
	@Status int = -1,
	@InspectionLocationId uniqueidentifier,
	@RemosId nvarchar(max)
AS

BEGIN
IF(@Status != -1)
	UPDATE LogisticsLocation set ApprovalStatus = case when (ApprovalStatus = 2) then 2 else @Status end WHERE RemosEstablishmentSchemeNumber = @RemosId;

update LogisticsLocation 
SET InspectionLocationId = @InspectionLocationId WHERE RemosEstablishmentSchemeNumber = @RemosId AND InspectionLocationId = '00000000-0000-0000-0000-000000000000'

END
