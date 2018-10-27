/*************************************************************************************************************************
*
*	File:			Destination.sql
*	Description:	Destination data
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Destination"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Destination"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Destination', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Destination"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Destination" (
			"CancelCustomFixTag" "varchar"(64) NULL ,
			"ClearingFirmId" "varchar"(64) NULL ,
			"DestinationId" "int" NOT NULL ,
			"ExternalId0" "varchar"(128) NULL ,
			"ExternalId1" "varchar"(128) NULL ,
			"FixAccount" "varchar"(64) NULL ,
			"FixAccountSuffixLength" "int" NULL ,
			"FixExchangeMnemonic" "varchar"(64) NULL ,
			"FixExecBroker" "varchar"(64) NULL ,
			"FixExecDestinationListed" "varchar"(64) NULL ,
			"FixExecDestinationUnlisted" "varchar"(64) NULL ,
			"FixHandleInstListed" "varchar"(64) NULL ,
			"FixHandleInstUnlisted" "varchar"(64) NULL ,
			"FixIoiRoutingId" "varchar"(64) NULL ,
			"FixIoiRoutingType" "varchar"(64) NULL ,
			"FixNetCommissionType" "varchar"(64) NULL ,
			"FixNoteTagId" "varchar"(64) NULL ,
			"FixOnBehalfOf" "varchar"(64) NULL ,
			"FixOnBehalfOfSub" "varchar"(64) NULL ,
			"FixOrderIdFormat" "varchar"(64) NOT NULL ,
			"FixPassiveTag" "varchar"(64) NULL ,
			"FixProactiveTag" "varchar"(64) NULL ,
			"FixSourceCompany" "varchar"(64) NULL ,
			"FixSourceId" "varchar"(64) NULL ,
			"FixSourceTrader" "varchar"(64) NULL ,
			"FixTargetCompany" "varchar"(64) NULL ,
			"FixTargetLocationId" "varchar"(64) NULL ,
			"FixTargetTrader" "varchar"(64) NULL ,
			"FixVersion" "varchar"(64) NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsBroker" "bit" NOT NULL ,
			"IsCancelAllowed" "bit" NOT NULL ,
			"IsCancelReplaceAllowed" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"IsEcn" "bit" NOT NULL ,
			"IsExchange" "bit" NOT NULL ,
			"IsInternal" "bit" NOT NULL ,
			"IsIoiAllowed" "bit" NOT NULL ,
			"IsLimitPricingAllowed" "bit" NOT NULL ,
			"IsMarketOnClosePricingAllowed" "bit" NOT NULL ,
			"IsMarketPricingAllowed" "bit" NOT NULL ,
			"IsSystemUp" "bit" NOT NULL ,
			"IsTagEligible" "bit" NULL ,
			"IsTradeAwayAllowed" "bit" NOT NULL ,
			"IsUnsolicitedTradeAllowed" "bit" NULL ,
			"LotSize" "int" NULL ,
			"MaximumQuantityListedLimit" "decimal"(32, 7) NULL ,
			"MaximumQuantityListedMarket" "decimal"(32, 7) NULL ,
			"MaximumQuantityUnlistedLimit" "decimal"(32, 7) NULL ,
			"MaximumQuantityUnlistedMarket" "decimal"(32, 7) NULL ,
			"ModifiedTime" "dateTime" NOT NULL ,
			"Name" "varchar"(64) NULL ,
			"NewOrderCustomFixTag" "varchar"(64) NULL ,
			"OatsDepartmentId" "varchar"(64) NULL ,
			"OatsDestinationCode" "varchar"(64) NULL ,
			"OatsReceivingMpi" "varchar"(64) NULL ,
			"OatsRouteMethod" "varchar"(64) NULL ,
			"OatsRoutingMpi" "varchar"(64) NULL ,
			"OatsSentMpi" "varchar"(64) NULL ,
			"OatsTerminalId" "varchar"(64) NULL ,
			"OnCloseCancelTime" "int" NULL ,
			"OnCloseOrderTime" "int" NULL ,
			"PerShareUsageFee" "decimal"(32, 7) NULL ,
			"PostMarketLogic" "varchar"(64) NULL ,
			"PrimaryBackupDestinationId" "int" NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"SecondaryBackupDestinationId" "int" NULL ,
			"SelectnetFlag" "bit" NOT NULL ,
			"ShortName" "varchar"(64) NOT NULL ,
			"SourceId0" "varchar"(64) NULL ,
			"SourceId1" "varchar"(64) NULL ,
			"SourceId2" "varchar"(64) NULL ,
			"SourceId3" "varchar"(64) NULL ,
			"SourceId4" "varchar"(64) NULL ,
			"SourceId5" "varchar"(64) NULL ,
			"SourceId6" "varchar"(64) NULL ,
			"SourceId7" "varchar"(64) NULL ,
			"UniqueShortName" "varchar"(64) NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Destination" WITH NOCHECK ADD 
			CONSTRAINT "PKDestination" PRIMARY KEY  NONCLUSTERED 
			(
				"DestinationId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Destination" WITH NOCHECK ADD 
			CONSTRAINT "DefaultDestinationIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultDestinationIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Destination''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Destination"
go
 