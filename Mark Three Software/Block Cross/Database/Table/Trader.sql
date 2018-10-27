/*************************************************************************************************************************
*
*	File:			Trader.sql
*	Description:	Contains information about broker where securities are traded.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Trader"
*	Description:	Contains information about the place where securites are traded.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Trader"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Trader', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Trader"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Trader" (
			"AccountIdDefault" "int" NULL,
			"Address1" "nvarchar"(64) NULL,
			"Address2" "nvarchar"(64) NULL,
			"Address3" "nvarchar"(64) NULL,
			"BlotterIdDefault" "int" NULL,
			"CommissionMaximum" "decimal"(32, 7) NULL,
			"CommissionMinimum" "decimal"(32, 7) NULL,
			"CommissionRate" "decimal"(32, 7) NULL,
			"CommissionRateTypeCode" "int" NULL,
			"EmailAddress" "nvarchar"(128) NULL ,
			"FixAccountId" "nvarchar"(64) NULL ,
			"IsAgencyMatch" "bit" NOT NULL ,
			"IsBrokerMatch" "bit" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsCommissionChangeAllowed" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"IsEditExecutionsAllowed" "bit" NOT NULL ,
			"IsHedgeMatch" "bit" NOT NULL ,
			"IsHeld" "bit" NOT NULL ,
			"IsInstitutionMatch" "bit" NOT NULL ,
			"LastDeletedOrder" "dateTime" NULL,
			"LastFilledOrder" "dateTime" NULL,
			"LastFilledSourceOrder" "dateTime" NULL,
			"LastTrade" "dateTime" NULL,
			"LotSizeDefault" "int" NOT NULL,
			"MarketSleep" "int" NOT NULL ,
			"MaximumVolatilityDefault" "decimal"(32, 7) NULL ,
			"NewsFreeTimeDefault" "int" NULL ,
			"OatsAccountType" "nvarchar"(64) NULL,
			"OatsOrigDeptId" "nvarchar"(64) NULL,
			"OrderMaximumLimitDelta" "decimal"(32, 7) NULL,
			"OrderMaximumMarketValue" "decimal"(32, 7) NULL,
			"OrderMaximumQuantity" "decimal"(32, 7) NULL,
			"OrderWarningMarketValue" "decimal"(32, 7) NULL,
			"OrderWarningQuantity" "decimal"(32, 7) NULL,
			"Phone" "nvarchar"(64) NULL,
			"ReviewWindow" "int" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"ScraperConfigurationString" "nvarchar"(255) NULL,
			"StartTimeDefault" "dateTime" NULL ,
			"StopTimeDefault" "dateTime" NULL ,
			"SubmissionTypeCode" "bit" NOT NULL ,
			"TagId" "nvarchar"(64) NULL,
			"TraderId" "int" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Trader" WITH NOCHECK ADD 
			CONSTRAINT "PKTrader" PRIMARY KEY  CLUSTERED 
			(
				"TraderId"
			)  ON "PRIMARY" 
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Trader" ADD 
			CONSTRAINT "FKObjectTrader" FOREIGN KEY 
			(
				"TraderId"
			) REFERENCES "Object" (
				"ObjectId"
			),
			CONSTRAINT "FKBlotterTrader" FOREIGN KEY 
			(
				"BlotterIdDefault"
			) REFERENCES "Blotter" (
				"BlotterId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* sDefault */

	execute ('
		ALTER TABLE "Trader" WITH NOCHECK ADD 
			CONSTRAINT "DefaultTraderIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultTraderIsDeleted" DEFAULT (0) FOR "IsDeleted",
			CONSTRAINT "IsCommissionChangeAllowDefault" DEFAULT (0) FOR "IsCommissionChangeAllowed",
			CONSTRAINT "IsEditExecutionsAllowedDefault" DEFAULT (0) FOR "IsEditExecutionsAllowed",
			CONSTRAINT "LotSizeDefault" DEFAULT (100) FOR "LotSizeDefault"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Trader''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Trader"
go
