/*************************************************************************************************************************
*
*	File:			TaxLot.sql
*	Description:	Information about individual purchases of security.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"TaxLot"
*	Description:	Tax lot contains individual trades in a given security, account and long/short position.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "TaxLot"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('TaxLot', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "TaxLot"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "TaxLot" (
			"TaxLotId" "int" NOT NULL ,
			"AccountId" "int" NOT NULL ,
			"SecurityId" "int" NOT NULL ,
			"PositionTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Quantity" "decimal"(15, 4) NOT NULL ,
			"Cost" "decimal"(15, 4) NOT NULL ,
			"LocalCost" "decimal"(15, 4) NOT NULL ,
			"TradeDate" "Datetime" NULL ,
			"SettlementDate" "Datetime" NULL ,
			"UserData0" "decimal"(15, 4) NULL ,
			"UserData1" "decimal"(15, 4) NULL ,
			"UserData2" "decimal"(15, 4) NULL ,
			"UserData3" "decimal"(15, 4) NULL ,
			"UserData4" "decimal"(15, 4) NULL ,
			"UserData5" "decimal"(15, 4) NULL ,
			"UserData6" "decimal"(15, 4) NULL ,
			"UserData7" "decimal"(15, 4) NULL ,
			"ExternalId0" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId2" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId3" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "TaxLot" WITH NOCHECK ADD 
			CONSTRAINT "PKTaxLot" PRIMARY KEY  NONCLUSTERED 
			(
				"TaxLotId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "TaxLot" ADD 
			CONSTRAINT "FKAccountTaxLot" FOREIGN KEY 
			(
				"AccountId"
			) REFERENCES "Account" (
				"AccountId"
			),
			CONSTRAINT "FKSecurityTaxLot" FOREIGN KEY 
			(
				"SecurityId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKPositionTypeTaxLot" FOREIGN KEY 
			(
				"PositionTypeCode"
			) REFERENCES "PositionType" (
				"PositionTypeCode"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "TaxLot" WITH NOCHECK ADD 
			CONSTRAINT "DefaultTaxLotIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultTaxLotIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''TaxLot''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end

/* Print the final state for the log. */

execute "ObjectEpilog" "TaxLot"
go
