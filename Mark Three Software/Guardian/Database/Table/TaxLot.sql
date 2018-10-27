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
			"AccountId" "int" NOT NULL ,
			"Cost" "decimal"(28, 7) NOT NULL ,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId2" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId3" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"LocalCost" "decimal"(28, 7) NOT NULL ,
			"PositionTypeCode" "int" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"Quantity" "decimal"(28, 7) NOT NULL ,
			"SecurityId" "int" NOT NULL ,
			"SettlementDate" "Datetime" NULL ,
			"TaxLotId" "int" NOT NULL ,
			"TradeDate" "Datetime" NULL ,
			"UserData0" "decimal"(28, 7) NULL ,
			"UserData1" "decimal"(28, 7) NULL ,
			"UserData2" "decimal"(28, 7) NULL ,
			"UserData3" "decimal"(28, 7) NULL ,
			"UserData4" "decimal"(28, 7) NULL ,
			"UserData5" "decimal"(28, 7) NULL ,
			"UserData6" "decimal"(28, 7) NULL ,
			"UserData7" "decimal"(28, 7) NULL
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
