/*************************************************************************************************************************
*
*	File:			AccountBase.sql
*	Description:	AccountBase data
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"AccountBase"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "AccountBase"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('AccountBase', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "AccountBase"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "AccountBase" (
			"AccountBaseId" "int" NOT NULL ,
			"Address0" "nvarchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Address1" "nvarchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"Address2" "nvarchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"City" "nvarchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"CountryId" "int" NULL ,
			"CurrencyId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Mnemonic" "nvarchar"(16) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"PostalCode" "varchar"(16) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ProvinceId" "int" NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"UserId" "int" NOT NULL ,
			"UserData0" "decimal"(32, 7) NULL ,
			"UserData1" "decimal"(32, 7) NULL ,
			"UserData2" "decimal"(32, 7) NULL ,
			"UserData3" "decimal"(32, 7) NULL ,
			"UserData4" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"UserData5" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"UserData6" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"UserData7" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "AccountBase" WITH NOCHECK ADD 
			CONSTRAINT "PKAccountBase" PRIMARY KEY  NONCLUSTERED 
			(
				"AccountBaseId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "AccountBase" ADD 
			CONSTRAINT "FKObjectAccountBase" FOREIGN KEY 
			(
				"AccountBaseId"
			) REFERENCES "Object" (
				"ObjectId"
			),
			CONSTRAINT "FKCurrencyAccountBase" FOREIGN KEY 
			(
				"CurrencyId"
			) REFERENCES "Currency" (
				"CurrencyId"
			),
			CONSTRAINT "FKCountryAccountBase" FOREIGN KEY 
			(
				"CountryId"
			) REFERENCES "Country" (
				"CountryId"
			),
			CONSTRAINT "FKProvinceAccountBase" FOREIGN KEY 
			(
				"ProvinceId"
			) REFERENCES "Province" (
				"ProvinceId"
			),
			CONSTRAINT "FKUserAccountBase" FOREIGN KEY 
			(
				"UserId"
			) REFERENCES "User" (
				"UserId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "AccountBase" WITH NOCHECK ADD 
			CONSTRAINT "DefaultAccountBaseIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultAccountBaseIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''AccountBase''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "AccountBase"
go
