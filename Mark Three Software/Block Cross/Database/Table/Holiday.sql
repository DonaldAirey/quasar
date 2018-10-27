/*************************************************************************************************************************
*
*	File:			Holiday.sql
*	Description:	Contains holiday information for a given country, securityType combination.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Holiday"
*	Description:	Contains information about holiday.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Holiday"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Holiday', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Holiday"
	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Holiday" (
			"CountryId" "int" NOT NULL ,
			"Date" "datetime" NOT NULL ,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"HolidayId" "int" NOT NULL ,
			"HolidayTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"Type" "varchar"(128) NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Holiday" WITH NOCHECK ADD 
			CONSTRAINT "PKHoliday" PRIMARY KEY  CLUSTERED 
			(
				"HolidayId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Holiday" ADD 
			CONSTRAINT "FKCountryHoliday" FOREIGN KEY 
			(
				"CountryId"
			) REFERENCES "Country" (
				"CountryId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Holiday" WITH NOCHECK ADD 
			CONSTRAINT "DefaultHolidayIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultHolidayIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Holiday''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Holiday"
go
