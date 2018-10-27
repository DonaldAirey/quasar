/*************************************************************************************************************************
*
*	File:			Country.sql
*	Description:	Lookup table for country information
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Country"
*	Description:	Contains information about country.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Country"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Country', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Country"
	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Country" (
			"Abbreviation" "nvarchar"(8) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"CountryId" "int" NOT NULL ,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Name" "nvarchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Country" WITH NOCHECK ADD 
			CONSTRAINT "PKCountry" PRIMARY KEY  CLUSTERED 
			(
				"CountryId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Country" WITH NOCHECK ADD 
			CONSTRAINT "DefaultCountryIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultCountryIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Country''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Country"
go
