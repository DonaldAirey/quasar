/*************************************************************************************************************************
*
*	File:			Province.sql
*	Description:	Defines Territories within countries.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Province"
*	Description:	Contains information about states and province within a country.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Province"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Province', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Province"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Province" (
			"ProvinceId" "int" NOT NULL ,
			"CountryId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Name" "nvarchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"Abbreviation" "nvarchar"(8) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"ExternalId0" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Clustered Index. */

	execute ('
		ALTER TABLE "Province" WITH NOCHECK ADD 
			CONSTRAINT "PKProvince" PRIMARY KEY  CLUSTERED 
			(
				"ProvinceId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Key */

	execute ('
		ALTER TABLE "Province" ADD 
			CONSTRAINT "FKCountryProvince" FOREIGN KEY 
			(
				"CountryId"
			) REFERENCES "Country" (
				"CountryId"
			)
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Province" WITH NOCHECK ADD 
			CONSTRAINT "DefaultProvinceIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultProvinceIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Province''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Province"
go
