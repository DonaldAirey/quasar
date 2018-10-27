/*************************************************************************************************************************
*
*	File:			Account.sql
*	Description:	Account data
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Account"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Account"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Account', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Account"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Account" (
			"AccountId" "int" NOT NULL ,
			"CurrencyId" "int" NOT NULL ,
			"SchemeId" "int" NULL ,
			"ModelId" "int" NULL ,
			"BlotterId" "int" NULL ,
			"StylesheetId" "int" NOT NULL ,
			"ProvinceId" "int" NULL ,
			"CountryId" "int" NULL ,
			"LotHandlingCode" "int" NULL ,
			"TimeInForceCode" "int" NULL ,
			"AccountTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Mnemonic" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Address0" "nvarchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Address1" "nvarchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Address2" "nvarchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"City" "nvarchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"PostalCode" "varchar"(16) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"UserData0" "decimal"(15, 4) NULL ,
			"UserData1" "decimal"(15, 4) NULL ,
			"UserData2" "decimal"(15, 4) NULL ,
			"UserData3" "decimal"(15, 4) NULL ,
			"UserData4" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"UserData5" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"UserData6" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"UserData7" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Account" WITH NOCHECK ADD 
			CONSTRAINT "PKAccount" PRIMARY KEY  NONCLUSTERED 
			(
				"AccountId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Account" ADD 
			CONSTRAINT "FKObjectAccount" FOREIGN KEY 
			(
				"AccountId"
			) REFERENCES "Object" (
				"ObjectId"
			),
			CONSTRAINT "FKCurrencyAccount" FOREIGN KEY 
			(
				"CurrencyId"
			) REFERENCES "Currency" (
				"CurrencyId"
			),
			CONSTRAINT "FKSchemeAccount" FOREIGN KEY 
			(
				"SchemeId"
			) REFERENCES "Scheme" (
				"SchemeId"
			),
			CONSTRAINT "FKModelAccount" FOREIGN KEY 
			(
				"ModelId"
			) REFERENCES "Model" (
				"ModelId"
			),
			CONSTRAINT "FKBlotterAccount" FOREIGN KEY 
			(
				"BlotterId"
			) REFERENCES "Blotter" (
				"BlotterId"
			),
			CONSTRAINT "FKCountryAccount" FOREIGN KEY 
			(
				"CountryId"
			) REFERENCES "Country" (
				"CountryId"
			),
			CONSTRAINT "FKLotHandlingAccount" FOREIGN KEY 
			(
				"LotHandlingCode"
			) REFERENCES "LotHandling" (
				"LotHandlingCode"
			),
			CONSTRAINT "FKProvinceAccount" FOREIGN KEY 
			(
				"ProvinceId"
			) REFERENCES "Province" (
				"ProvinceId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Account" WITH NOCHECK ADD 
			CONSTRAINT "DefaultAccountIsIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultAccountIsIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Account''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Account"
go
